using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface IPurchaseRepository
{
    Task<int> CreatePurchaseWithDetailsAsync(Purchase purchase, IEnumerable<PurchaseDetail> details);
    Task<dynamic> GetPurchaseWithDetailsAsync(int id);
    Task<BillingSystem.Domain.Models.PagedResult<dynamic>> GetPagedAsync(string search, int page, int pageSize);
}
