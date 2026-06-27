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
        using var connection = _conn.CreateConnection();
        var sql = "UPDATE Categories SET Name = @Name, Description = @Description, UpdatedAt = CURRENT_TIMESTAMP WHERE Id = @Id;";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<BillingSystem.Domain.Models.PagedResult<Category>> GetPagedAsync(string search, int page, int pageSize)
    {
        using var connection = _conn.CreateConnection();
        var searchPattern = $"%{search}%";
        var offset = (page - 1) * pageSize;
        var limit = pageSize > 0 ? pageSize : 10;
        
        var baseSql = "FROM Categories WHERE IsActive = TRUE";
        if (!string.IsNullOrWhiteSpace(search))
        {
            baseSql += " AND (Name ILIKE @Search OR Description ILIKE @Search)";
        }
        
        var countSql = $"SELECT COUNT(*) {baseSql}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { Search = searchPattern });
        
        var dataSql = $"SELECT * {baseSql} ORDER BY Id DESC LIMIT @Limit OFFSET @Offset";
        var items = await connection.QueryAsync<Category>(dataSql, new { Search = searchPattern, Limit = limit, Offset = offset });
        
        return new BillingSystem.Domain.Models.PagedResult<Category>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
