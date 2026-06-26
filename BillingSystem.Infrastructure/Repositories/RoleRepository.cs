using System.Data;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public RoleRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Role?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Role>("SELECT * FROM Roles WHERE Id = @Id AND IsActive = TRUE", new { Id = id });
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Role>("SELECT * FROM Roles WHERE IsActive = TRUE");
    }

    public async Task<int> AddAsync(Role entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"INSERT INTO Roles (Name, Description, CreatedAt, IsActive) 
                    VALUES (@Name, @Description, @CreatedAt, @IsActive) RETURNING Id;";
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task<int> UpdateAsync(Role entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"UPDATE Roles SET Name = @Name, Description = @Description, UpdatedAt = @UpdatedAt, IsActive = @IsActive WHERE Id = @Id;";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<IEnumerable<Permission>> GetAllPermissionsAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Permission>("SELECT * FROM Permissions WHERE IsActive = TRUE ORDER BY Module, DisplayName");
    }

    public async Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(int roleId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"SELECT p.* FROM Permissions p 
                    INNER JOIN RolePermissions rp ON p.Id = rp.PermissionId 
                    WHERE rp.RoleId = @RoleId AND p.IsActive = TRUE";
        return await connection.QueryAsync<Permission>(sql, new { RoleId = roleId });
    }

    public async Task AssignPermissionsToRoleAsync(int roleId, IEnumerable<int> permissionIds)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            // Delete existing
            await connection.ExecuteAsync("DELETE FROM RolePermissions WHERE RoleId = @RoleId", new { RoleId = roleId }, transaction);

            // Insert new
            if (permissionIds.Any())
            {
                var sql = "INSERT INTO RolePermissions (RoleId, PermissionId) VALUES (@RoleId, @PermissionId)";
                foreach (var permId in permissionIds)
                {
                    await connection.ExecuteAsync(sql, new { RoleId = roleId, PermissionId = permId }, transaction);
                }
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
