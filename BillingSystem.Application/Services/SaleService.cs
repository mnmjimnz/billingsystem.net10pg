using System.Transactions;
using BillingSystem.Application.DTOs;
using BillingSystem.Application.Interfaces;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;

namespace BillingSystem.Application.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepo;
    private readonly IProductRepository _productRepo;
    private readonly IKardexRepository _kardexRepo;
    private readonly IReceivableRepository _receivableRepo;
    private readonly INotificationRepository _notificationRepo;
    private readonly INotificationService _notificationService;
    private readonly ICashRegisterRepository _cashRepo;
    private readonly ISettingsRepository _settingsRepo;
    private readonly IAccountingService _accountingService;

    public SaleService(
        ISaleRepository saleRepo,
        IProductRepository productRepo,
        IKardexRepository kardexRepo,
        IReceivableRepository receivableRepo,
        INotificationRepository notificationRepo,
        INotificationService notificationService,
        ICashRegisterRepository cashRepo,
        ISettingsRepository settingsRepo,
        IAccountingService accountingService)
    {
        _saleRepo = saleRepo;
        _productRepo = productRepo;
        _kardexRepo = kardexRepo;
        _receivableRepo = receivableRepo;
        _notificationRepo = notificationRepo;
        _notificationService = notificationService;
        _cashRepo = cashRepo;
        _settingsRepo = settingsRepo;
        _accountingService = accountingService;
    }

    public async Task<(int SaleId, string TicketNumber)> CreateSaleAsync(CreateSaleRequest request, int userId, int branchId)
    {
        var session = await _cashRepo.GetActiveSessionAsync(userId);
        if (session == null) throw new Exception("Debe aperturar su caja antes de poder realizar ventas.");

        var currentSalesTotal = await _saleRepo.GetSessionSalesTotalAsync(userId, session.OpeningTime);
        var physicalCash = session.OpeningBalance + currentSalesTotal;

        if (request.PaymentType != "CREDIT" && request.Change > physicalCash)
            throw new Exception($"La caja no cuenta con suficiente dinero físico para dar el cambio. Efectivo disponible: ${physicalCash:F2}. Solicite asignación de saldo al encargado.");

        var settings = await _settingsRepo.GetSettingsAsync();
        decimal taxPercentage = settings.TaxPercentage / 100m;
        
        // Calculate tax based on request details
        decimal taxAmount = 0;
        foreach (var detail in request.Details)
        {
            var p = await _productRepo.GetByIdAsync(detail.ProductId);
            if (p != null && !p.IsTaxExempt)
            {
                // Assuming detail.Subtotal is the sum of items before tax.
                // Tax is applied ON TOP of the subtotal for this item.
                taxAmount += detail.Subtotal * taxPercentage;
            }
        }

        var sale = new Sale
        {
            TicketNumber = "TKT-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
            CustomerId = request.CustomerId ?? 1,
            UserId = userId == 0 ? 1 : userId,
            BranchId = branchId == 0 ? 1 : branchId,
            Subtotal = request.Subtotal,
            Discount = request.Discount,
            TaxAmount = taxAmount,
            Total = request.Subtotal - request.Discount + taxAmount,
            PaymentType = request.PaymentType,
            AmountTendered = request.AmountTendered,
            Change = request.Change,
            Status = request.PaymentType == "CREDIT" ? "PENDING" : "PAID"
        };

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        
        // Check stock availability BEFORE creating the sale
        foreach (var detail in request.Details)
        {
            var product = await _productRepo.GetByIdAsync(detail.ProductId);
            if (product == null) throw new Exception($"Producto con ID {detail.ProductId} no encontrado.");
            
            var branchStock = await _productRepo.GetStockForBranchAsync(detail.ProductId, branchId);
            if (branchStock < detail.Quantity)
                throw new Exception($"Existencias insuficientes para el producto '{product.Name}' en esta sucursal. Stock disponible: {branchStock}");
        }

        // 1. Insert Sale & Details
        var saleId = await _saleRepo.CreateSaleWithDetailsAsync(sale, request.Details);

        decimal totalCostOfGoodsSold = 0;

        // 2. Update Stock and Kardex
        foreach (var detail in request.Details)
        {
            var product = await _productRepo.GetByIdAsync(detail.ProductId);
            if (product != null)
            {
                totalCostOfGoodsSold += detail.Quantity * product.Cost;

                // Log Movement
                var movement = new InventoryMovement
                {
                    ProductId = detail.ProductId,
                    MovementType = "OUT",
                    ReferenceType = "SALE",
                    ReferenceId = saleId,
                    BranchId = sale.BranchId,
                    Quantity = detail.Quantity,
                    PreviousStock = await _productRepo.GetStockForBranchAsync(detail.ProductId, sale.BranchId) + detail.Quantity,
                    NewStock = await _productRepo.GetStockForBranchAsync(detail.ProductId, sale.BranchId),
                    Description = "Venta desde POS"
                };
                await _kardexRepo.AddMovementAsync(movement);

                // Update Stock
                await _productRepo.UpdateStockForBranchAsync(detail.ProductId, sale.BranchId, -detail.Quantity);
            }
        }

        // 3. Handle Credit Sales
        if (sale.PaymentType == "CREDIT")
        {
            var initialBalance = sale.Total - sale.AmountTendered;
            if (initialBalance < 0) initialBalance = 0;

            var account = new AccountsReceivable
            {
                SaleId = saleId,
                CustomerId = sale.CustomerId,
                TotalDebt = sale.Total,
                Balance = initialBalance,
                DueDate = DateTime.UtcNow.AddDays(30),
                Status = initialBalance <= 0 ? "PAID" : "PENDING"
            };
            var arId = await _receivableRepo.CreateAsync(account);

            if (sale.AmountTendered > 0)
            {
                var payment = new ReceivablePayment
                {
                    AccountId = arId,
                    UserId = sale.UserId,
                    Amount = sale.AmountTendered,
                    Notes = "Abono inicial en punto de venta"
                };
                await _receivableRepo.AddPaymentAsync(payment);
            }

            if (initialBalance > 0)
            {
                var msg = $"Se generó una venta al crédito por ${sale.Total} al cliente ID {sale.CustomerId}.";
                var notif = new Notification
                {
                    Title = "Venta al Crédito",
                    Message = msg,
                    Type = "CREDIT_SALE",
                    ReferenceId = arId
                };
                await _notificationRepo.AddAsync(notif);
                await _notificationService.DispatchNotificationAsync(notif.Title, notif.Message, notif.Type, arId); 
            }
        }

        scope.Complete();
        
        // 4. Registrar en contabilidad (Póliza contable de venta)
        // Se ejecuta fuera del TransactionScope para evitar el error de transacciones anidadas
        await _accountingService.RecordPosSaleAsync(sale, totalCostOfGoodsSold);

        return (saleId, sale.TicketNumber);
    }
}
