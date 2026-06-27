using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class ReceivableRepository : IReceivableRepository
{
    private readonly DbConnectionFactory _db;
    public ReceivableRepository(DbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<dynamic>> GetReceivablesAsync()
    {
        using var connection = _db.CreateConnection();
        var sql = @"SELECT a.*, c.Name as CustomerName, s.TicketNumber
                    FROM AccountsReceivable a 
                    JOIN Customers c ON a.CustomerId = c.Id
                    JOIN Sales s ON a.SaleId = s.Id
                    WHERE a.IsActive = TRUE ORDER BY a.CreatedAt DESC";
        return await connection.QueryAsync(sql);
    }

    public async Task<AccountsReceivable?> GetByIdAsync(int id)
    {
        using var connection = _db.CreateConnection();
        var sql = "SELECT * FROM AccountsReceivable WHERE Id = @Id";
        return await connection.QuerySingleOrDefaultAsync<AccountsReceivable>(sql, new { Id = id });
    }

    public async Task UpdateAccountAndAddPaymentAsync(AccountsReceivable account, ReceivablePayment payment)
    {
        using var connection = _db.CreateConnection();
        var paySql = @"INSERT INTO ReceivablePayments (AccountId, UserId, Amount, Notes) 
                       VALUES (@AccountId, @UserId, @Amount, @Notes)";
        await connection.ExecuteAsync(paySql, payment);

        var updateAccSql = "UPDATE AccountsReceivable SET AmountPaid = @AmountPaid, Balance = @Balance, Status = @Status WHERE Id = @Id";
        await connection.ExecuteAsync(updateAccSql, account);
    }

    public async Task<int> CreateAsync(AccountsReceivable account)
    {
        using var connection = _db.CreateConnection();
        var arSql = @"INSERT INTO AccountsReceivable (SaleId, CustomerId, TotalDebt, Balance, DueDate, Status) 
                      VALUES (@SaleId, @CustomerId, @TotalDebt, @Balance, @DueDate, @Status) RETURNING Id;";
        return await connection.ExecuteScalarAsync<int>(arSql, account);
    }

    public async Task AddPaymentAsync(ReceivablePayment payment)
    {
        using var connection = _db.CreateConnection();
        var paySql = @"INSERT INTO ReceivablePayments (AccountId, UserId, Amount, Notes) 
                       VALUES (@AccountId, @UserId, @Amount, @Notes)";
        await connection.ExecuteAsync(paySql, payment);
    }

    public async Task<BillingSystem.Domain.Models.PagedResult<dynamic>> GetPagedAsync(string search, int page, int pageSize)
    {
        using var connection = _db.CreateConnection();
        var searchPattern = $"%{search}%";
        var offset = (page - 1) * pageSize;
        var limit = pageSize > 0 ? pageSize : 10;
        
        var baseSql = @"FROM AccountsReceivable a 
                        JOIN Customers c ON a.CustomerId = c.Id
                        JOIN Sales s ON a.SaleId = s.Id
                        WHERE a.IsActive = TRUE";
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            baseSql += " AND (a.Status ILIKE @Search OR c.Name ILIKE @Search OR s.TicketNumber ILIKE @Search)";
        }
        
        var countSql = $"SELECT COUNT(*) {baseSql}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { Search = searchPattern });
        
        var dataSql = $"SELECT a.*, c.Name as CustomerName, s.TicketNumber {baseSql} ORDER BY a.CreatedAt DESC LIMIT @Limit OFFSET @Offset";
        var items = await connection.QueryAsync<dynamic>(dataSql, new { Search = searchPattern, Limit = limit, Offset = offset });
        
        return new BillingSystem.Domain.Models.PagedResult<dynamic>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<IEnumerable<ReceivablePayment>> GetPaymentsAsync(int accountId)
    {
        using var connection = _db.CreateConnection();
        var sql = "SELECT * FROM ReceivablePayments WHERE AccountId = @AccountId ORDER BY PaymentDate DESC";
        return await connection.QueryAsync<ReceivablePayment>(sql, new { AccountId = accountId });
    }
}
