using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface IBranchMovementRepository : IRepository<BranchMovement>
{
    Task<IEnumerable<BranchMovement>> GetByBranchIdAsync(int branchId);
    Task<BillingSystem.Domain.Models.PagedResult<BranchMovement>> GetPagedByBranchIdAsync(int branchId, int page, int pageSize);
    Task<decimal> GetSessionMovementsTotalAsync(int cashRegisterId, DateTime openingTime, string type);
}
