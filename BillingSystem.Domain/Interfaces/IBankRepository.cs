using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface IBankRepository
{
    Task<IEnumerable<BankAccount>> GetBankAccountsAsync();
    Task<BankAccount?> GetBankAccountByIdAsync(int id);
    Task<int> AddBankAccountAsync(BankAccount entity);
    Task<int> UpdateBankAccountAsync(BankAccount entity);
    
    Task<IEnumerable<BankReconciliation>> GetReconciliationsAsync(int bankAccountId);
    Task<BankReconciliation?> GetReconciliationByIdAsync(int id);
    Task<int> AddReconciliationAsync(BankReconciliation entity, IEnumerable<BankReconciliationDetail> details);
    Task<int> UpdateReconciliationStatusAsync(int id, string status);
}
