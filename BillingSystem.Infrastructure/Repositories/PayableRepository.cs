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

    public async Task<BillingSystem.Domain.Models.PagedResult<dynamic>> GetPagedAsync(string search, int page, int pageSize)
    {
        using var connection = _db.CreateConnection();
        var searchPattern = $"%{search}%";
        var offset = (page - 1) * pageSize;
        var limit = pageSize > 0 ? pageSize : 10;
        
        var baseSql = @"FROM AccountsPayable a
                        JOIN Suppliers s ON a.SupplierId = s.Id
                        WHERE a.IsActive = TRUE";
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            baseSql += " AND (a.Status ILIKE @Search OR s.Name ILIKE @Search)";
        }
        
        var countSql = $"SELECT COUNT(*) {baseSql}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { Search = searchPattern });
        
        var dataSql = $"SELECT a.*, s.Name as SupplierName {baseSql} ORDER BY a.CreatedAt DESC LIMIT @Limit OFFSET @Offset";
        var items = await connection.QueryAsync<dynamic>(dataSql, new { Search = searchPattern, Limit = limit, Offset = offset });
        
        return new BillingSystem.Domain.Models.PagedResult<dynamic>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<IEnumerable<PayablePayment>> GetPaymentsAsync(int accountId)
    {
        using var connection = _db.CreateConnection();
        var sql = "SELECT * FROM PayablePayments WHERE AccountId = @AccountId ORDER BY PaymentDate DESC";
        return await connection.QueryAsync<PayablePayment>(sql, new { AccountId = accountId });
    }
}
