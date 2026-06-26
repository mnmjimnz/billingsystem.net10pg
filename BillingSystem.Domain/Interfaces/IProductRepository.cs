using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByBarcodeAsync(string barcode);
    Task UpdateStockAsync(int productId, int quantityChange);
    Task UpdateStockAndCostAsync(int productId, int quantityChange, decimal newCost);
}
