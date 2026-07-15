using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface IStockTransferRepository
{
    Task<int> AddTransferAsync(StockTransfer transfer);
    Task<IEnumerable<StockTransfer>> GetAllTransfersAsync(int? branchId = null);
    Task<BillingSystem.Domain.Models.PagedResult<StockTransfer>> GetPagedAsync(int page, int pageSize, int? branchId = null);
}
