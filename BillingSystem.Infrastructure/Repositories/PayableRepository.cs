using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class PayableRepository : IPayableRepository
{
    private readonly DbConnectionFactory _db;

    public PayableRepository(DbConnectionFactory db) => _db = db;

    public async Task<int> CreateAccountAsync(AccountsPayable account)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            INSERT INTO AccountsPayable (PurchaseId, SupplierId, TotalDebt, AmountPaid, Balance, DueDate, Status)
            VALUES (@PurchaseId, @SupplierId, @TotalDebt, @AmountPaid, @Balance, @DueDate, @Status)
            RETURNING Id;";
        return await connection.ExecuteScalarAsync<int>(sql, account);
    }

    public async Task<IEnumerable<AccountsPayable>> GetPendingAsync()
    {
        using var connection = _db.CreateConnection();
        var sql = "SELECT * FROM AccountsPayable WHERE Status = 'PENDING' AND IsActive = TRUE ORDER BY CreatedAt DESC";
        return await connection.QueryAsync<AccountsPayable>(sql);
    }

    public async Task<AccountsPayable?> GetAccountByIdAsync(int id)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<AccountsPayable>("SELECT * FROM AccountsPayable WHERE Id = @Id", new { Id = id });
    }

    public async Task<int> AddPaymentAsync(PayablePayment payment)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            INSERT INTO PayablePayments (AccountId, UserId, Amount, Notes)
            VALUES (@AccountId, @UserId, @Amount, @Notes)
            RETURNING Id;";
        return await connection.ExecuteScalarAsync<int>(sql, payment);
    }

    public async Task<int> UpdateAccountBalanceAsync(int accountId, decimal amountPaid)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            UPDATE AccountsPayable 
            SET AmountPaid = AmountPaid + @AmountPaid,
                Balance = Balance - @AmountPaid,
                Status = CASE WHEN (Balance - @AmountPaid) <= 0 THEN 'PAID' ELSE 'PENDING' END,
                UpdatedAt = CURRENT_TIMESTAMP
            WHERE Id = @Id;";
        return await connection.ExecuteAsync(sql, new { Id = accountId, AmountPaid = amountPaid });
    }
}
