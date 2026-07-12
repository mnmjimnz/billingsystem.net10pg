using System.Data;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class BankRepository : IBankRepository
{
    private readonly DbConnectionFactory _db;

    public BankRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<IEnumerable<BankAccount>> GetBankAccountsAsync()
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<BankAccount>("SELECT * FROM BankAccounts WHERE IsActive = TRUE");
    }

    public async Task<BankAccount?> GetBankAccountByIdAsync(int id)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<BankAccount>("SELECT * FROM BankAccounts WHERE Id = @Id AND IsActive = TRUE", new { Id = id });
    }

    public async Task<int> AddBankAccountAsync(BankAccount entity)
    {
        using var connection = _db.CreateConnection();
        var sql = @"INSERT INTO BankAccounts (BankName, AccountNumber, Currency, CurrentBalance, LinkedAccountId) 
                    VALUES (@BankName, @AccountNumber, @Currency, @CurrentBalance, @LinkedAccountId) RETURNING Id;";
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task<int> UpdateBankAccountAsync(BankAccount entity)
    {
        using var connection = _db.CreateConnection();
        var sql = @"UPDATE BankAccounts SET BankName = @BankName, AccountNumber = @AccountNumber, Currency = @Currency, 
                    CurrentBalance = @CurrentBalance, LinkedAccountId = @LinkedAccountId, UpdatedAt = CURRENT_TIMESTAMP 
                    WHERE Id = @Id;";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<IEnumerable<BankReconciliation>> GetReconciliationsAsync(int bankAccountId)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<BankReconciliation>(
            "SELECT * FROM BankReconciliations WHERE BankAccountId = @BankAccountId AND IsActive = TRUE ORDER BY StatementDate DESC", 
            new { BankAccountId = bankAccountId });
    }

    public async Task<BankReconciliation?> GetReconciliationByIdAsync(int id)
    {
        using var connection = _db.CreateConnection();
        var rec = await connection.QueryFirstOrDefaultAsync<BankReconciliation>("SELECT * FROM BankReconciliations WHERE Id = @Id", new { Id = id });
        return rec;
    }

    public async Task<int> AddReconciliationAsync(BankReconciliation entity, IEnumerable<BankReconciliationDetail> details)
    {
        using var connection = _db.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            var sqlRec = @"INSERT INTO BankReconciliations (BankAccountId, StatementDate, StatementBalance, Status, Notes) 
                           VALUES (@BankAccountId, @StatementDate, @StatementBalance, @Status, @Notes) RETURNING Id;";
            var recId = await connection.ExecuteScalarAsync<int>(sqlRec, entity, transaction);

            var sqlDetail = @"INSERT INTO BankReconciliationDetails (BankReconciliationId, JournalEntryDetailId, IsCleared) 
                              VALUES (@BankReconciliationId, @JournalEntryDetailId, @IsCleared);";
                              
            foreach (var detail in details)
            {
                detail.BankReconciliationId = recId;
                await connection.ExecuteAsync(sqlDetail, detail, transaction);
            }

            transaction.Commit();
            return recId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<int> UpdateReconciliationStatusAsync(int id, string status)
    {
        using var connection = _db.CreateConnection();
        var sql = @"UPDATE BankReconciliations SET Status = @Status, UpdatedAt = CURRENT_TIMESTAMP WHERE Id = @Id;";
        return await connection.ExecuteAsync(sql, new { Id = id, Status = status });
    }
}
