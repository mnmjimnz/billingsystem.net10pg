using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface IPurchaseRepository
{
    Task<int> CreatePurchaseWithDetailsAsync(Purchase purchase, IEnumerable<PurchaseDetail> details);
}
