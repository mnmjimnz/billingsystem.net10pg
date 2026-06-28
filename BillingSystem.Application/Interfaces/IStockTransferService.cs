using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Models;

namespace BillingSystem.Application.Interfaces;

public interface IStockTransferService
{
    Task<int> TransferStockAsync(StockTransfer transfer);
    Task<IEnumerable<StockTransfer>> GetAllTransfersAsync();
}
