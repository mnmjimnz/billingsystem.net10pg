using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface IBranchRepository : IRepository<Branch>
{
    Task UpdateStatusAsync(int id, string status);
    Task UpdateFundsAsync(int id, decimal amount);
}
