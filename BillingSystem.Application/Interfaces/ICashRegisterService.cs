using BillingSystem.Domain.Entities;

namespace BillingSystem.Application.Interfaces;

public interface ICashRegisterService
{
    Task<CashRegisterSession?> GetActiveSessionAsync(int userId);
    Task<int> OpenSessionAsync(int userId, int cashRegisterId, decimal openingBalance);
    Task CloseSessionAsync(int userId);
    Task<object?> GetSessionSummaryAsync(int userId);
    Task<IEnumerable<CashRegister>> GetRegistersByBranchAsync(int branchId);
}
