using System.Transactions;
using BillingSystem.Application.DTOs;
using BillingSystem.Application.Interfaces;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;

namespace BillingSystem.Application.Services;

public class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _purchaseRepo;
    private readonly IProductRepository _productRepo;
    private readonly IKardexRepository _kardexRepo;
    private readonly IPayableRepository _payableRepo;
    private readonly INotificationRepository _notificationRepo;
    private readonly INotificationService _notificationService;
    private readonly IBranchRepository _branchRepo;
    private readonly IAccountingService _accountingService;
    private readonly IBranchMovementService _branchMovementService;

    public PurchaseService(
        IPurchaseRepository purchaseRepo,
        IProductRepository productRepo,
        IKardexRepository kardexRepo,
        IPayableRepository payableRepo,
        INotificationRepository notificationRepo,
        INotificationService notificationService,
        IBranchRepository branchRepo,
        IAccountingService accountingService,
        IBranchMovementService branchMovementService)
    {
        _purchaseRepo = purchaseRepo;
        _productRepo = productRepo;
        _kardexRepo = kardexRepo;
        _payableRepo = payableRepo;
        _notificationRepo = notificationRepo;
        _notificationService = notificationService;
        _branchRepo = branchRepo;
        _accountingService = accountingService;
        _branchMovementService = branchMovementService;
    }

    public async Task<int> CreatePurchaseAsync(PurchaseDto dto, int userId)
    {
        var branch = await _branchRepo.GetByIdAsync(dto.BranchId);
        if (branch == null) throw new Exception("La sucursal seleccionada no existe.");
        if (branch.Status != "OPEN") throw new Exception("No se pueden realizar compras porque la sucursal está CERRADA.");

        if (dto.AmountPaid > 0)
        {
            if (branch.AvailableFunds < dto.AmountPaid)
            {
                throw new Exception($"Fondos insuficientes en la sucursal. Disponible: ${branch.AvailableFunds}, Requerido: ${dto.AmountPaid}");
            }
            // Eliminamos la resta manual, delegándola a BranchMovementService
        }

        var purchase = new Purchase
        {
            InvoiceNumber = dto.InvoiceNumber,
            SupplierId = dto.SupplierId,
            UserId = userId,
            BranchId = dto.BranchId,
            Total = dto.Total,
            PaymentType = dto.PaymentType,
            AmountPaid = dto.AmountPaid,
            Status = dto.PaymentType == "CREDIT" ? "PENDING" : "PAID"
        };

        if (dto.AmountPaid > 0)
        {
            var movement = new BranchMovement
            {
                BranchId = dto.BranchId,
                Amount = dto.AmountPaid,
                Type = "OUT",
                Category = "Compras",
                Description = $"Abono a compra {dto.InvoiceNumber}",
                UserId = userId
            };
            await _branchMovementService.RegisterMovementAsync(movement);
        }

        var purchaseId = await _purchaseRepo.CreatePurchaseWithDetailsAsync(purchase, dto.Details);

        foreach (var detail in dto.Details)
        {
            var product = await _productRepo.GetByIdAsync(detail.ProductId);
            if (product != null)
            {
                await _productRepo.UpdateStockAndCostForBranchAsync(detail.ProductId, purchase.BranchId, detail.Quantity, detail.UnitCost);

                var movement = new InventoryMovement
                {
                    ProductId = detail.ProductId,
                    MovementType = "IN",
                    ReferenceType = "PURCHASE",
                    ReferenceId = purchaseId,
                    BranchId = purchase.BranchId,
                    Quantity = detail.Quantity,
                    PreviousStock = await _productRepo.GetStockForBranchAsync(detail.ProductId, purchase.BranchId) - detail.Quantity,
                    NewStock = await _productRepo.GetStockForBranchAsync(detail.ProductId, purchase.BranchId),
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
            await _notificationService.DispatchNotificationAsync(notification.Title, notification.Message, notification.Type, purchaseId);
        }

        await _accountingService.RecordPurchaseAsync(purchase);

        return purchaseId;
    }

    public async Task<dynamic> GetPurchaseWithDetailsAsync(int id)
    {
        return await _purchaseRepo.GetPurchaseWithDetailsAsync(id);
    }

    public async Task<BillingSystem.Domain.Models.PagedResult<dynamic>> GetPagedAsync(string search, int page, int pageSize, int? branchId = null)
    {
        return await _purchaseRepo.GetPagedAsync(search, page, pageSize, branchId);
    }
}
