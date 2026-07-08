using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Models;

namespace BillingSystem.Application.Interfaces;

public interface IBranchMovementService
{
    Task<Result<BranchMovement>> RegisterMovementAsync(BranchMovement movement);
    Task<IEnumerable<BranchMovement>> GetMovementsByBranchIdAsync(int branchId);
    Task<BillingSystem.Domain.Models.PagedResult<BranchMovement>> GetPagedMovementsByBranchIdAsync(int branchId, int page, int pageSize);
}
