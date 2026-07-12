using System.Data;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class AccountingRepository : IAccountingRepository
{
    private readonly DbConnectionFactory _db;

    public AccountingRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Account>> GetAccountsAsync()
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<Account>("SELECT * FROM Accounts WHERE IsActive = TRUE ORDER BY Code");
    }

    public async Task<Account?> GetAccountByIdAsync(int id)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Account>("SELECT * FROM Accounts WHERE Id = @Id AND IsActive = TRUE", new { Id = id });
    }

    public async Task<int> AddAccountAsync(Account entity)
    {
        using var connection = _db.CreateConnection();
        var sql = @"INSERT INTO Accounts (Code, Name, Type, ParentAccountId, Level, AllowsTransactions, Description) 
                    VALUES (@Code, @Name, @Type, @ParentAccountId, @Level, @AllowsTransactions, @Description) RETURNING Id;";
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task<int> UpdateAccountAsync(Account entity)
    {
        using var connection = _db.CreateConnection();
        var sql = @"UPDATE Accounts SET Code = @Code, Name = @Name, Type = @Type, ParentAccountId = @ParentAccountId, 
                    Level = @Level, AllowsTransactions = @AllowsTransactions, Description = @Description, UpdatedAt = CURRENT_TIMESTAMP 
                    WHERE Id = @Id;";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<IEnumerable<JournalEntry>> GetJournalEntriesAsync(DateTime? startDate, DateTime? endDate)
    {
        using var connection = _db.CreateConnection();
        var sql = "SELECT * FROM JournalEntries WHERE IsActive = TRUE";
        
        if (startDate.HasValue) sql += " AND Date >= @StartDate";
        if (endDate.HasValue) sql += " AND Date <= @EndDate";
        
        sql += " ORDER BY Date DESC, Id DESC LIMIT 100";
        
        return await connection.QueryAsync<JournalEntry>(sql, new { StartDate = startDate, EndDate = endDate });
    }

    public async Task<JournalEntry?> GetJournalEntryByIdAsync(int id)
    {
        using var connection = _db.CreateConnection();
        var entry = await connection.QueryFirstOrDefaultAsync<JournalEntry>("SELECT * FROM JournalEntries WHERE Id = @Id", new { Id = id });
        if (entry != null)
        {
            var details = await connection.QueryAsync<JournalEntryDetail>("SELECT * FROM JournalEntryDetails WHERE JournalEntryId = @Id", new { Id = id });
            entry.Details = details.ToList();
        }
        return entry;
    }

    public async Task<int> AddJournalEntryAsync(JournalEntry entry, IEnumerable<JournalEntryDetail> details)
    {
        using var connection = _db.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            var sqlEntry = @"INSERT INTO JournalEntries (Date, Description, ReferenceType, ReferenceId) 
                             VALUES (@Date, @Description, @ReferenceType, @ReferenceId) RETURNING Id;";
            var entryId = await connection.ExecuteScalarAsync<int>(sqlEntry, entry, transaction);

            var sqlDetail = @"INSERT INTO JournalEntryDetails (JournalEntryId, AccountId, AccountCode, AccountName, Debit, Credit) 
                              VALUES (@JournalEntryId, @AccountId, @AccountCode, @AccountName, @Debit, @Credit);";
                              
            foreach (var detail in details)
            {
                detail.JournalEntryId = entryId;
                await connection.ExecuteAsync(sqlDetail, detail, transaction);
            }

            transaction.Commit();
            return entryId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<IEnumerable<dynamic>> GetAccountLedgerAsync(int accountId, DateTime startDate, DateTime endDate)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            SELECT je.Date, je.Description as EntryDescription, je.ReferenceType, je.ReferenceId,
                   jed.Debit, jed.Credit
            FROM JournalEntryDetails jed
            JOIN JournalEntries je ON jed.JournalEntryId = je.Id
            WHERE jed.AccountId = @AccountId 
              AND je.Date >= @StartDate AND je.Date <= @EndDate
              AND je.IsActive = TRUE AND jed.IsActive = TRUE
            ORDER BY je.Date ASC, je.Id ASC
        ";
        return await connection.QueryAsync<dynamic>(sql, new { AccountId = accountId, StartDate = startDate, EndDate = endDate });
    }

    public async Task<IEnumerable<dynamic>> GetTrialBalanceAsync(DateTime startDate, DateTime endDate)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            SELECT a.Id, a.Code, a.Name, a.Type,
                   COALESCE(SUM(CASE WHEN je.Date < @StartDate THEN jed.Debit - jed.Credit ELSE 0 END), 0) as InitialBalance,
                   COALESCE(SUM(CASE WHEN je.Date >= @StartDate AND je.Date <= @EndDate THEN jed.Debit ELSE 0 END), 0) as PeriodDebit,
                   COALESCE(SUM(CASE WHEN je.Date >= @StartDate AND je.Date <= @EndDate THEN jed.Credit ELSE 0 END), 0) as PeriodCredit
            FROM Accounts a
            LEFT JOIN JournalEntryDetails jed ON a.Id = jed.AccountId AND jed.IsActive = TRUE
            LEFT JOIN JournalEntries je ON jed.JournalEntryId = je.Id AND je.IsActive = TRUE AND je.Date <= @EndDate
            WHERE a.IsActive = TRUE
            GROUP BY a.Id, a.Code, a.Name, a.Type
            HAVING 
                COALESCE(SUM(CASE WHEN je.Date < @StartDate THEN jed.Debit - jed.Credit ELSE 0 END), 0) != 0 OR
                COALESCE(SUM(CASE WHEN je.Date >= @StartDate AND je.Date <= @EndDate THEN jed.Debit ELSE 0 END), 0) != 0 OR
                COALESCE(SUM(CASE WHEN je.Date >= @StartDate AND je.Date <= @EndDate THEN jed.Credit ELSE 0 END), 0) != 0
            ORDER BY a.Code
        ";
        return await connection.QueryAsync<dynamic>(sql, new { StartDate = startDate, EndDate = endDate });
    }
}
