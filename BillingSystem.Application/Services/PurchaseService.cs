using System.Transactions;
using BillingSystem.Application.DTOs;
using BillingSystem.Application.Interfaces;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;
using BillingSystem.API.Hubs;

namespace BillingSystem.Application.Services;

public class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _purchaseRepo;
    private readonly IProductRepository _productRepo;
    private readonly IKardexRepository _kardexRepo;
    private readonly IPayableRepository _payableRepo;
    private readonly INotificationRepository _notificationRepo;
    private readonly IHubContext<NotificationHub> _hubContext;

    public PurchaseService(
        IPurchaseRepository purchaseRepo,
        IProductRepository productRepo,
        IKardexRepository kardexRepo,
        IPayableRepository payableRepo,
        INotificationRepository notificationRepo,
        IHubContext<NotificationHub> hubContext)
    {
        _purchaseRepo = purchaseRepo;
        _productRepo = productRepo;
        _kardexRepo = kardexRepo;
        _payableRepo = payableRepo;
        _notificationRepo = notificationRepo;
        _hubContext = hubContext;
    }

    public async Task<int> CreatePurchaseAsync(PurchaseDto dto, int userId)
    {
        var purchase = new Purchase
        {
            InvoiceNumber = dto.InvoiceNumber,
            SupplierId = dto.SupplierId,
            UserId = userId,
            Total = dto.Total,
            PaymentType = dto.PaymentType,
            AmountPaid = dto.AmountPaid,
            Status = dto.PaymentType == "CREDIT" ? "PENDING" : "PAID"
        };

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var purchaseId = await _purchaseRepo.CreatePurchaseWithDetailsAsync(purchase, dto.Details);

        foreach (var detail in dto.Details)
        {
            var product = await _productRepo.GetByIdAsync(detail.ProductId);
            if (product != null)
            {
                await _productRepo.UpdateStockAndCostAsync(detail.ProductId, detail.Quantity, detail.UnitCost);

                var movement = new InventoryMovement
                {
                    ProductId = detail.ProductId,
                    MovementType = "IN",
                    ReferenceType = "PURCHASE",
                    ReferenceId = purchaseId,
                    Quantity = detail.Quantity,
                    PreviousStock = product.Stock,
                    NewStock = product.Stock + detail.Quantity,
                    Description = $"Compra {purchase.InvoiceNumber}"
                };
                await _kardexRepo.AddMovementAsync(movement);
            }
        }

        if (dto.PaymentType == "CREDIT")
        {
            var payable = new AccountsPayable
            {
                PurchaseId = purchaseId,
                SupplierId = dto.SupplierId,
                TotalDebt = dto.Total,
                AmountPaid = dto.AmountPaid,
                Balance = dto.Total - dto.AmountPaid,
                DueDate = DateTime.UtcNow.AddDays(30) // Assuming 30 days credit
            };

            await _payableRepo.CreateAccountAsync(payable);

            var notification = new Notification
            {
                Title = "Compra al Crédito a Pagar",
                Message = $"Se ha registrado una nueva cuenta por pagar de la compra {dto.InvoiceNumber} por ${payable.Balance}.",
                Type = "WARNING",
                ReferenceId = purchaseId
            };

            await _notificationRepo.AddAsync(notification);
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification.Title, notification.Message);
        }

        scope.Complete();
        return purchaseId;
    }
}
