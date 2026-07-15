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

    public async Task<IEnumerable<StockTransfer>> GetAllTransfersAsync(int? branchId = null)
    {
        using var connection = _db.CreateConnection();
        var condition = branchId.HasValue ? "WHERE st.FromBranchId = @BranchId OR st.ToBranchId = @BranchId" : "";
        var sql = $@"
            SELECT st.*, p.Id, p.Name, fb.Id, fb.Name, tb.Id, tb.Name, u.Id, u.FullName
            FROM StockTransfers st
            JOIN Products p ON st.ProductId = p.Id
            JOIN Branches fb ON st.FromBranchId = fb.Id
            JOIN Branches tb ON st.ToBranchId = tb.Id
            JOIN Users u ON st.UserId = u.Id
            {condition}
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
            new { BranchId = branchId }
        );
    }

    public async Task<BillingSystem.Domain.Models.PagedResult<StockTransfer>> GetPagedAsync(int page, int pageSize, int? branchId = null)
    {
        using var connection = _db.CreateConnection();
        var offset = (page - 1) * pageSize;
        
        var condition = branchId.HasValue ? "WHERE st.FromBranchId = @BranchId OR st.ToBranchId = @BranchId" : "";
        
        var countSql = $"SELECT COUNT(*) FROM StockTransfers st {condition}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { BranchId = branchId });
        
        var sql = $@"
            SELECT st.*, p.Id, p.Name, fb.Id, fb.Name, tb.Id, tb.Name, u.Id, u.FullName
            FROM StockTransfers st
            JOIN Products p ON st.ProductId = p.Id
            JOIN Branches fb ON st.FromBranchId = fb.Id
            JOIN Branches tb ON st.ToBranchId = tb.Id
            JOIN Users u ON st.UserId = u.Id
            {condition}
            ORDER BY st.CreatedAt DESC
            LIMIT @Limit OFFSET @Offset";
            
        var items = await connection.QueryAsync<StockTransfer, Product, Branch, Branch, User, StockTransfer>(
            sql,
            (st, p, fb, tb, u) => 
            {
                st.Product = p;
                st.FromBranch = fb;
                st.ToBranch = tb;
                st.User = u;
                return st;
            },
            new { Limit = pageSize, Offset = offset, BranchId = branchId });
        
        return new BillingSystem.Domain.Models.PagedResult<StockTransfer>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
