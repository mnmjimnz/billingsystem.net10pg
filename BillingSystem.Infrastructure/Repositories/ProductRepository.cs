using System.Data;
using Dapper;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;

namespace BillingSystem.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DbConnectionFactory _db;

    public ProductRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<Product>("SELECT * FROM Products ORDER BY Name");
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Product>("SELECT * FROM Products WHERE Id = @Id", new { Id = id });
    }

    
    public async Task<Product?> GetByBarcodeAsync(string barcode)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Product>("SELECT * FROM Products WHERE Barcode = @Barcode", new { Barcode = barcode });
    }

    public async Task<int> AddAsync(Product entity)
    {
        using var connection = _db.CreateConnection();
        var sql = @"INSERT INTO Products (Barcode, Name, Price, Cost, Stock, CategoryId, IsTaxExempt, CreatedAt, UpdatedAt)
                    VALUES (@Barcode, @Name, @Price, @Cost, @Stock, @CategoryId, @IsTaxExempt, @CreatedAt, @UpdatedAt) RETURNING Id";
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task<int> UpdateAsync(Product entity)
    {
        using var connection = _db.CreateConnection();
        var sql = @"UPDATE Products SET 
                    Barcode = @Barcode, Name = @Name, Price = @Price, Cost = @Cost, 
                    Stock = @Stock, CategoryId = @CategoryId, IsTaxExempt = @IsTaxExempt, UpdatedAt = @UpdatedAt
                    WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var connection = _db.CreateConnection();
        return await connection.ExecuteAsync("DELETE FROM Products WHERE Id = @Id", new { Id = id });
    }

    public async Task UpdateStockAsync(int productId, int quantityChange)
    {
        using var connection = _db.CreateConnection();
        var sql = "UPDATE Products SET Stock = Stock + @QuantityChange WHERE Id = @Id";
        await connection.ExecuteAsync(sql, new { QuantityChange = quantityChange, Id = productId });
    }

    public async Task UpdateStockAndCostAsync(int productId, int quantityChange, decimal newCost)
    {
        using var connection = _db.CreateConnection();
        var sql = @"UPDATE Products 
                    SET Stock = Stock + @QuantityChange, Cost = @NewCost, UpdatedAt = CURRENT_TIMESTAMP
                    WHERE Id = @Id";
        await connection.ExecuteAsync(sql, new { QuantityChange = quantityChange, NewCost = newCost, Id = productId });
    }

    // --- NEW MULTI-BRANCH STOCK METHODS ---
    public async Task UpdateStockForBranchAsync(int productId, int branchId, int quantityChange)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            INSERT INTO ProductStocks (ProductId, BranchId, Stock, CreatedAt, UpdatedAt)
            VALUES (@ProductId, @BranchId, @QuantityChange, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
            ON CONFLICT (ProductId, BranchId) 
            DO UPDATE SET Stock = ProductStocks.Stock + @QuantityChange, UpdatedAt = CURRENT_TIMESTAMP;
        ";
        await connection.ExecuteAsync(sql, new { ProductId = productId, BranchId = branchId, QuantityChange = quantityChange });
    }
    
    public async Task UpdateStockAndCostForBranchAsync(int productId, int branchId, int quantityChange, decimal newCost)
    {
        using var connection = _db.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            // 1. Update Product global cost
            var sql1 = "UPDATE Products SET Cost = @NewCost, UpdatedAt = CURRENT_TIMESTAMP WHERE Id = @Id";
            await connection.ExecuteAsync(sql1, new { NewCost = newCost, Id = productId }, transaction);
            
            // 2. Update branch-specific stock
            var sql2 = @"
                INSERT INTO ProductStocks (ProductId, BranchId, Stock, CreatedAt, UpdatedAt)
                VALUES (@ProductId, @BranchId, @QuantityChange, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
                ON CONFLICT (ProductId, BranchId) 
                DO UPDATE SET Stock = ProductStocks.Stock + @QuantityChange, UpdatedAt = CURRENT_TIMESTAMP;
            ";
            await connection.ExecuteAsync(sql2, new { ProductId = productId, BranchId = branchId, QuantityChange = quantityChange }, transaction);
            
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<int> GetStockForBranchAsync(int productId, int branchId)
    {
        using var connection = _db.CreateConnection();
        var sql = "SELECT Stock FROM ProductStocks WHERE ProductId = @ProductId AND BranchId = @BranchId";
        return await connection.QuerySingleOrDefaultAsync<int>(sql, new { ProductId = productId, BranchId = branchId });
    }

    public async Task<IEnumerable<dynamic>> GetStockByBranchAsync(int productId)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            SELECT ps.BranchId, b.Name as BranchName, ps.Stock
            FROM ProductStocks ps
            JOIN Branches b ON ps.BranchId = b.Id
            WHERE ps.ProductId = @ProductId
            ORDER BY b.Name
        ";
        return await connection.QueryAsync(sql, new { ProductId = productId });
    }
    // --------------------------------------

    public async Task<BillingSystem.Domain.Models.PagedResult<Product>> GetPagedAsync(string search, int page, int pageSize)
    {
        using var connection = _db.CreateConnection();
        var searchCondition = string.IsNullOrWhiteSpace(search) ? "" : "WHERE Name ILIKE @Search OR Barcode ILIKE @Search";
        
        // Now, we need to return the TOTAL stock across all branches, or at least the global stock.
        // For backwards compatibility before we drop the global stock column, we can return the global stock.
        // Wait, if purchases/sales ONLY update branch stock, the global stock won't change.
        // Let's dynamically sum the branch stock.
        var sql = $@"
            SELECT p.*, c.Name as CategoryName,
                   COALESCE((SELECT SUM(Stock) FROM ProductStocks ps WHERE ps.ProductId = p.Id), 0) as CalculatedTotalStock
            FROM Products p
            LEFT JOIN Categories c ON p.CategoryId = c.Id
            {searchCondition}
            ORDER BY p.Name
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            
            SELECT COUNT(*) FROM Products
            {searchCondition};
        ";

        using var multi = await connection.QueryMultipleAsync(sql, new 
        { 
            Search = $"%{search}%", 
            Offset = (page - 1) * pageSize, 
            PageSize = pageSize 
        });

        var items = await multi.ReadAsync<dynamic>();
        var totalCount = await multi.ReadFirstAsync<int>();

        return new BillingSystem.Domain.Models.PagedResult<Product>
        {
            Items = items.Select(i => new Product 
            { 
                Id = i.id,
                Barcode = i.barcode,
                Name = i.name,
                Price = i.price,
                Cost = i.cost,
                Stock = (int)i.calculatedtotalstock,
                CategoryId = i.categoryid,
                IsTaxExempt = i.istaxexempt
            }),
            TotalCount = totalCount
        };
    }

    public async Task<IEnumerable<Product>> GetByCategoriesAsync(IEnumerable<int> categoryIds)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<Product>("SELECT * FROM Products WHERE CategoryId = ANY(@CategoryIds)", new { CategoryIds = categoryIds.ToArray() });
    }
}
