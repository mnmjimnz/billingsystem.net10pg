using System.Data;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public UserRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id AND IsActive = TRUE", new { Id = id });
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<User>("SELECT * FROM Users WHERE IsActive = TRUE");
    }

    public async Task<int> AddAsync(User entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"INSERT INTO Users (Username, PasswordHash, FullName, RoleId, BranchId, CreatedAt, IsActive) 
                    VALUES (@Username, @PasswordHash, @FullName, @RoleId, @BranchId, @CreatedAt, @IsActive) RETURNING Id;";
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task<int> UpdateAsync(User entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"UPDATE Users SET Username = @Username, FullName = @FullName, RoleId = @RoleId, BranchId = @BranchId, UpdatedAt = @UpdatedAt, IsActive = @IsActive WHERE Id = @Id;";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Username = @Username AND IsActive = TRUE", new { Username = username });
    }
}
