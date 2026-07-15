using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Models;

namespace BillingSystem.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByBarcodeAsync(string barcode);
    Task UpdateStockAsync(int productId, int quantityChange);
    Task UpdateStockAndCostAsync(int productId, int quantityChange, decimal newCost);
    
    // NEW MULTI-BRANCH STOCK METHODS
    Task UpdateStockForBranchAsync(int productId, int branchId, int quantityChange);
    Task UpdateStockAndCostForBranchAsync(int productId, int branchId, int quantityChange, decimal newCost);
    Task<int> GetStockForBranchAsync(int productId, int branchId);
    Task<IEnumerable<dynamic>> GetStockByBranchAsync(int productId);
    
    // Additional methods that might exist
    Task<PagedResult<Product>> GetPagedAsync(string search, int page, int pageSize, int? branchId = null);
    Task<IEnumerable<Product>> GetByCategoriesAsync(IEnumerable<int> categoryIds);
}
