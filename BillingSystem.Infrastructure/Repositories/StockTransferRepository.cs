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
            SELECT st.*, p.Name, fb.Name, tb.Name, u.FullName
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
            splitOn: "Name,Name,Name,FullName"
        );
    }

    public async Task<BillingSystem.Domain.Models.PagedResult<StockTransfer>> GetPagedAsync(int page, int pageSize)
    {
        using var connection = _db.CreateConnection();
        var offset = (page - 1) * pageSize;
        
        var countSql = "SELECT COUNT(*) FROM StockTransfers";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql);
        
        var dataSql = @"
            SELECT st.*, 
                   fb.Name as FromBranchName,
                   tb.Name as ToBranchName,
                   p.Name as ProductName
            FROM StockTransfers st
            JOIN Branches fb ON st.FromBranchId = fb.Id
            JOIN Branches tb ON st.ToBranchId = tb.Id
            JOIN Products p ON st.ProductId = p.Id
            ORDER BY st.TransferDate DESC
            LIMIT @Limit OFFSET @Offset";
            
        var items = await connection.QueryAsync<StockTransfer>(dataSql, new { Limit = pageSize, Offset = offset });
        
        return new BillingSystem.Domain.Models.PagedResult<StockTransfer>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
