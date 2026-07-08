using BillingSystem.Application.Interfaces;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;

namespace BillingSystem.Application.Services;

public class StockTransferService : IStockTransferService
{
    private readonly IStockTransferRepository _transferRepo;
    private readonly IProductRepository _productRepo;
    private readonly IKardexRepository _kardexRepo;

    public StockTransferService(
        IStockTransferRepository transferRepo,
        IProductRepository productRepo,
        IKardexRepository kardexRepo)
    {
        _transferRepo = transferRepo;
        _productRepo = productRepo;
        _kardexRepo = kardexRepo;
    }

    public async Task<int> TransferStockAsync(StockTransfer transfer)
    {
        // 1. Verify enough stock in source branch
        var currentSourceStock = await _productRepo.GetStockForBranchAsync(transfer.ProductId, transfer.FromBranchId);
        if (currentSourceStock < transfer.Quantity)
        {
            throw new Exception("Existencias insuficientes en la sucursal de origen.");
        }

        // 2. Reduce stock in source branch
        await _productRepo.UpdateStockForBranchAsync(transfer.ProductId, transfer.FromBranchId, -transfer.Quantity);

        // 3. Add stock to destination branch
        await _productRepo.UpdateStockForBranchAsync(transfer.ProductId, transfer.ToBranchId, transfer.Quantity);

        // 4. Record Transfer
        var transferId = await _transferRepo.AddTransferAsync(transfer);

        // 5. Add Kardex movements
        await _kardexRepo.AddMovementAsync(new InventoryMovement
        {
            ProductId = transfer.ProductId,
            BranchId = transfer.FromBranchId,
            MovementType = "OUT",
            ReferenceType = "TRANSFER",
            ReferenceId = transferId,
            Quantity = transfer.Quantity,
            PreviousStock = currentSourceStock,
            NewStock = currentSourceStock - transfer.Quantity,
            Description = $"Traslado hacia sucursal ID: {transfer.ToBranchId}. Notas: {transfer.Notes}"
        });

        var currentDestStock = await _productRepo.GetStockForBranchAsync(transfer.ProductId, transfer.ToBranchId);
        await _kardexRepo.AddMovementAsync(new InventoryMovement
        {
            ProductId = transfer.ProductId,
            BranchId = transfer.ToBranchId,
            MovementType = "IN",
            ReferenceType = "TRANSFER",
            ReferenceId = transferId,
            Quantity = transfer.Quantity,
            PreviousStock = currentDestStock - transfer.Quantity,
            NewStock = currentDestStock,
            Description = $"Traslado desde sucursal ID: {transfer.FromBranchId}. Notas: {transfer.Notes}"
        });

        return transferId;
    }

    public async Task<IEnumerable<StockTransfer>> GetAllTransfersAsync()
    {
        return await _transferRepo.GetAllTransfersAsync();
    }

    public async Task<BillingSystem.Domain.Models.PagedResult<StockTransfer>> GetPagedTransfersAsync(int page, int pageSize)
    {
        return await _transferRepo.GetPagedAsync(page, pageSize);
    }
}
