using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface IAccountingRepository
{
    Task<IEnumerable<Account>> GetAccountsAsync();
    Task<Account?> GetAccountByIdAsync(int id);
    Task<int> AddAccountAsync(Account entity);
    Task<int> UpdateAccountAsync(Account entity);
    
    Task<IEnumerable<JournalEntry>> GetJournalEntriesAsync(DateTime? startDate, DateTime? endDate);
    Task<JournalEntry?> GetJournalEntryByIdAsync(int id);
    Task<int> AddJournalEntryAsync(JournalEntry entry, IEnumerable<JournalEntryDetail> details);
    
    // Ledgers & balances
    Task<IEnumerable<dynamic>> GetAccountLedgerAsync(int accountId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<dynamic>> GetTrialBalanceAsync(DateTime startDate, DateTime endDate);
}
