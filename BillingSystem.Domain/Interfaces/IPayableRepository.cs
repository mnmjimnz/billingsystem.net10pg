using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface IPayableRepository
{
    Task<int> CreateAccountAsync(AccountsPayable account);
    Task<IEnumerable<AccountsPayable>> GetPendingAsync();
    Task<int> AddPaymentAsync(PayablePayment payment);
    Task<AccountsPayable?> GetAccountByIdAsync(int id);
    Task<int> UpdateAccountBalanceAsync(int accountId, decimal amountPaid);
}
