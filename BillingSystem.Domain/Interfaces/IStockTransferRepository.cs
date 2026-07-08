using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface IStockTransferRepository
{
    Task<int> AddTransferAsync(StockTransfer transfer);
    Task<IEnumerable<StockTransfer>> GetAllTransfersAsync();
    Task<BillingSystem.Domain.Models.PagedResult<StockTransfer>> GetPagedAsync(int page, int pageSize);
}
