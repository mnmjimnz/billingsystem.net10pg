using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Models;

namespace BillingSystem.Domain.Interfaces;

public interface ICashRegisterRepository
{
    Task<CashRegisterSession?> GetActiveSessionAsync(int userId);
    Task<int> OpenSessionAsync(CashRegisterSession session);
    Task CloseSessionAsync(CashRegisterSession session);
    Task<CashRegister?> GetDefaultRegisterAsync(int branchId);
    Task<CashRegister?> GetByIdAsync(int id);
    Task<IEnumerable<CashRegister>> GetByBranchIdAsync(int branchId);
    Task<int> AddAsync(CashRegister register);
    Task<int> UpdateAsync(CashRegister register);
    Task UpdateStatusAsync(int id, bool isActive);
}
