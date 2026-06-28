using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface IStockTransferRepository
{
    Task<int> AddTransferAsync(StockTransfer transfer);
    Task<IEnumerable<StockTransfer>> GetAllTransfersAsync();
}
