using BillingSystem.Domain.Entities;

namespace BillingSystem.Application.Interfaces;

public interface ICashRegisterService
{
    Task<CashRegisterSession?> GetActiveSessionAsync(int userId);
    Task<int> OpenSessionAsync(int userId, int branchId, decimal openingBalance);
    Task CloseSessionAsync(int userId, decimal declaredBalance);
}
