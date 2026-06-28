using Dapper;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;

namespace BillingSystem.Infrastructure.Repositories;

public class StockTransferRepository : IStockTransferRepository
{
    private readonly DbConnectionFactory _db;

    public StockTransferRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<int> AddTransferAsync(StockTransfer transfer)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            INSERT INTO StockTransfers (ProductId, FromBranchId, ToBranchId, Quantity, UserId, CreatedAt, Notes)
            VALUES (@ProductId, @FromBranchId, @ToBranchId, @Quantity, @UserId, CURRENT_TIMESTAMP, @Notes)
            RETURNING Id;
        ";
        return await connection.ExecuteScalarAsync<int>(sql, transfer);
    }

    public async Task<IEnumerable<StockTransfer>> GetAllTransfersAsync()
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            SELECT st.*, p.Name as ProductName, fb.Name as FromBranchName, tb.Name as ToBranchName, u.FullName as UserName
            FROM StockTransfers st
            JOIN Products p ON st.ProductId = p.Id
            JOIN Branches fb ON st.FromBranchId = fb.Id
            JOIN Branches tb ON st.ToBranchId = tb.Id
            JOIN Users u ON st.UserId = u.Id
            ORDER BY st.CreatedAt DESC
        ";
        
        return await connection.QueryAsync<StockTransfer, Product, Branch, Branch, User, StockTransfer>(
            sql,
            (st, p, fb, tb, u) => 
            {
                st.Product = p;
                st.FromBranch = fb;
                st.ToBranch = tb;
                st.User = u;
                return st;
            },
            splitOn: "ProductName,FromBranchName,ToBranchName,UserName"
        );
    }
}
