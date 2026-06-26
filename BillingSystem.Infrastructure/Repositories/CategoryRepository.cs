using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly DbConnectionFactory _conn;
    public CategoryRepository(DbConnectionFactory conn) => _conn = conn;

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        using var db = _conn.CreateConnection();
        return await db.QueryAsync<Category>("SELECT * FROM Categories WHERE IsActive = TRUE");
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        using var db = _conn.CreateConnection();
        return await db.QueryFirstOrDefaultAsync<Category>("SELECT * FROM Categories WHERE Id = @Id AND IsActive = TRUE", new { Id = id });
    }

    public async Task<int> AddAsync(Category entity)
    {
        using var db = _conn.CreateConnection();
        var sql = "INSERT INTO Categories (Name, Description, CreatedAt, IsActive) VALUES (@Name, @Description, @CreatedAt, @IsActive) RETURNING Id;";
        return await db.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task<int> UpdateAsync(Category entity)
    {
        using var db = _conn.CreateConnection();
        var sql = "UPDATE Categories SET Name = @Name, Description = @Description, UpdatedAt = @UpdatedAt, IsActive = @IsActive WHERE Id = @Id;";
        return await db.ExecuteAsync(sql, entity);
    }
}
