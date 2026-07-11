namespace BillingSystem.Domain.Interfaces;
using BillingSystem.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

public interface ICouponRepository : IRepository<Coupon>
{
    Task<Coupon?> GetByCodeAsync(string code);
    Task<IEnumerable<Coupon>> GetAllActiveAsync();
    Task<int> DeleteAsync(int id);
}
