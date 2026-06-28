using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly DbConnectionFactory _db;
    public PurchaseRepository(DbConnectionFactory db) => _db = db;

    public async Task<int> CreatePurchaseWithDetailsAsync(Purchase purchase, IEnumerable<PurchaseDetail> details)
    {
        using var connection = _db.CreateConnection();
        var purchaseSql = @"
            INSERT INTO Purchases (InvoiceNumber, SupplierId, UserId, BranchId, Total, PaymentType, AmountPaid, Status, CreatedAt, IsActive)
            VALUES (@InvoiceNumber, @SupplierId, @UserId, @BranchId, @Total, @PaymentType, @AmountPaid, @Status, CURRENT_TIMESTAMP, TRUE)
            RETURNING Id;";
        
        var purchaseId = await connection.ExecuteScalarAsync<int>(purchaseSql, purchase);

        foreach (var detail in details)
        {
            detail.PurchaseId = purchaseId;
            
            var detailSql = @"
                INSERT INTO PurchaseDetails (PurchaseId, ProductId, Quantity, UnitCost, Subtotal, CreatedAt, IsActive)
                VALUES (@PurchaseId, @ProductId, @Quantity, @UnitCost, @Subtotal, CURRENT_TIMESTAMP, TRUE);";
            await connection.ExecuteAsync(detailSql, detail);
        }

        return purchaseId;
    }

    public async Task<dynamic> GetPurchaseWithDetailsAsync(int id)
    {
        using var connection = _db.CreateConnection();
        var purchaseSql = @"SELECT p.*, s.Name as SupplierName, u.FullName as UserName, b.Name as BranchName 
                            FROM Purchases p 
                            JOIN Suppliers s ON p.SupplierId = s.Id
                            JOIN Users u ON p.UserId = u.Id
                            JOIN Branches b ON p.BranchId = b.Id
                            WHERE p.Id = @Id";
        var purchase = await connection.QueryFirstOrDefaultAsync<dynamic>(purchaseSql, new { Id = id });

        if (purchase != null)
        {
            var detailsSql = @"SELECT pd.*, pr.Name as ProductName, pr.Code as ProductCode 
                               FROM PurchaseDetails pd
                               JOIN Products pr ON pd.ProductId = pr.Id
                               WHERE pd.PurchaseId = @Id";
            var details = await connection.QueryAsync<dynamic>(detailsSql, new { Id = id });
            
            // Assigning details to a new dynamic object to return together
            return new {
                Purchase = purchase,
                Details = details
            };
        }
        return null;
    }

    public Task<Purchase?> GetByIdAsync(int id) => throw new NotImplementedException();
    public Task<IEnumerable<Purchase>> GetAllAsync() => throw new NotImplementedException();
    public Task<int> AddAsync(Purchase entity) => throw new NotImplementedException();
    public Task<int> UpdateAsync(Purchase entity) => throw new NotImplementedException();

    public async Task<BillingSystem.Domain.Models.PagedResult<dynamic>> GetPagedAsync(string search, int page, int pageSize)
    {
        using var connection = _db.CreateConnection();
        var searchPattern = $"%{search}%";
        var offset = (page - 1) * pageSize;
        var limit = pageSize > 0 ? pageSize : 10;
        
        var baseSql = @"FROM Purchases p 
                        JOIN Suppliers s ON p.SupplierId = s.Id
                        JOIN Users u ON p.UserId = u.Id
                        JOIN Branches b ON p.BranchId = b.Id
                        WHERE p.IsActive = TRUE";
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            baseSql += " AND (p.InvoiceNumber ILIKE @Search OR p.PaymentType ILIKE @Search OR p.Status ILIKE @Search OR s.Name ILIKE @Search OR u.FullName ILIKE @Search)";
        }
        
        var countSql = $"SELECT COUNT(*) {baseSql}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { Search = searchPattern });
        
        var dataSql = $"SELECT p.*, s.Name as SupplierName, u.FullName as UserName, b.Name as BranchName {baseSql} ORDER BY p.CreatedAt DESC LIMIT @Limit OFFSET @Offset";
        var items = await connection.QueryAsync<dynamic>(dataSql, new { Search = searchPattern, Limit = limit, Offset = offset });
        
        return new BillingSystem.Domain.Models.PagedResult<dynamic>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
