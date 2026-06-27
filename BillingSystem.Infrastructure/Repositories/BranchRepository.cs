using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class BranchRepository : IBranchRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public BranchRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Branch?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Branch>("SELECT * FROM Branches WHERE Id = @Id AND IsActive = TRUE", new { Id = id });
    }

    public async Task<IEnumerable<Branch>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Branch>("SELECT * FROM Branches WHERE IsActive = TRUE");
    }

    public async Task<int> AddAsync(Branch entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"INSERT INTO Branches (Name, Address, Phone, AvailableFunds, CreatedAt, IsActive) 
                    VALUES (@Name, @Address, @Phone, @AvailableFunds, @CreatedAt, @IsActive) RETURNING Id;";
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task<int> UpdateAsync(Branch entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"UPDATE Branches 
                    SET Name = @Name, 
                        Address = @Address, 
                        Phone = @Phone, 
                        Status = @Status,
                        AvailableFunds = @AvailableFunds,
                        UpdatedAt = CURRENT_TIMESTAMP 
                    WHERE Id = @Id;";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task UpdateStatusAsync(int id, string status)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "UPDATE Branches SET Status = @Status, UpdatedAt = CURRENT_TIMESTAMP WHERE Id = @Id",
            new { Status = status, Id = id });
    }

    public async Task UpdateFundsAsync(int id, decimal amount)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "UPDATE Branches SET AvailableFunds = AvailableFunds + @Amount, UpdatedAt = CURRENT_TIMESTAMP WHERE Id = @Id",
            new { Amount = amount, Id = id });
    }

    public async Task<BillingSystem.Domain.Models.PagedResult<Branch>> GetPagedAsync(string search, int page, int pageSize)
    {
        using var connection = _connectionFactory.CreateConnection();
        var searchPattern = $"%{search}%";
        var offset = (page - 1) * pageSize;
        var limit = pageSize > 0 ? pageSize : 10;
        
        var baseSql = "FROM Branches WHERE IsActive = TRUE";
        if (!string.IsNullOrWhiteSpace(search))
        {
            baseSql += " AND (Name ILIKE @Search OR Address ILIKE @Search OR Phone ILIKE @Search)";
        }
        
        var countSql = $"SELECT COUNT(*) {baseSql}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { Search = searchPattern });
        
        var dataSql = $"SELECT * {baseSql} ORDER BY Id DESC LIMIT @Limit OFFSET @Offset";
        var items = await connection.QueryAsync<Branch>(dataSql, new { Search = searchPattern, Limit = limit, Offset = offset });
        
        return new BillingSystem.Domain.Models.PagedResult<Branch>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
