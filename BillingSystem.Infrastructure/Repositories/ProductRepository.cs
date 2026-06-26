using System.Data;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DbConnectionFactory _db;
    public ProductRepository(DbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<Product>("SELECT * FROM Products WHERE IsActive = TRUE");
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Product>("SELECT * FROM Products WHERE Id = @Id", new { Id = id });
    }

    public async Task<Product?> GetByBarcodeAsync(string barcode)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Product>("SELECT * FROM Products WHERE Barcode = @Barcode AND IsActive = TRUE", new { Barcode = barcode });
    }

    public async Task<int> AddAsync(Product entity)
    {
        using var connection = _db.CreateConnection();
        var sql = @"INSERT INTO Products (Name, Description, Barcode, CategoryId, Cost, Price, Stock, CreatedAt, IsActive) 
                    VALUES (@Name, @Description, @Barcode, @CategoryId, @Cost, @Price, @Stock, @CreatedAt, @IsActive) RETURNING Id;";
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task<int> UpdateAsync(Product entity)
    {
        using var connection = _db.CreateConnection();
        var sql = @"UPDATE Products SET Name = @Name, Description = @Description, Barcode = @Barcode, 
                    CategoryId = @CategoryId, Cost = @Cost, Price = @Price, Stock = @Stock,
                    IsActive = @IsActive, UpdatedAt = CURRENT_TIMESTAMP WHERE Id = @Id;";
        return await connection.ExecuteAsync(sql, entity);
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
}
