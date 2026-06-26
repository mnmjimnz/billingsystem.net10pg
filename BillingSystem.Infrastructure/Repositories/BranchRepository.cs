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
        var sql = @"INSERT INTO Branches (Name, Address, Phone, CreatedAt, IsActive) 
                    VALUES (@Name, @Address, @Phone, @CreatedAt, @IsActive) RETURNING Id;";
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task<int> UpdateAsync(Branch entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"UPDATE Branches SET Name = @Name, Address = @Address, Phone = @Phone, UpdatedAt = @UpdatedAt, IsActive = @IsActive WHERE Id = @Id;";
        return await connection.ExecuteAsync(sql, entity);
    }
}
