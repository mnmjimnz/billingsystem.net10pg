using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface ISaleRepository : IRepository<Sale> 
{
    Task<int> CreateSaleWithDetailsAsync(Sale sale, IEnumerable<SaleDetail> details);
    Task<decimal> GetSessionSalesTotalAsync(int userId, DateTime since);
}
