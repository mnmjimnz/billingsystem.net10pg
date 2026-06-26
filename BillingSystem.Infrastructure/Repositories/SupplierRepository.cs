using System.Data;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public SupplierRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Supplier?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Supplier>("SELECT * FROM Suppliers WHERE Id = @Id AND IsActive = TRUE", new { Id = id });
    }

    public async Task<IEnumerable<Supplier>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Supplier>("SELECT * FROM Suppliers WHERE IsActive = TRUE");
    }

    public async Task<int> AddAsync(Supplier entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"INSERT INTO Suppliers (Name, DocumentNumber, ContactName, Email, Phone, Address, CreatedAt, IsActive) 
                    VALUES (@Name, @DocumentNumber, @ContactName, @Email, @Phone, @Address, @CreatedAt, @IsActive) RETURNING Id;";
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task<int> UpdateAsync(Supplier entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"UPDATE Suppliers SET Name = @Name, DocumentNumber = @DocumentNumber, ContactName = @ContactName, Email = @Email, Phone = @Phone, Address = @Address, UpdatedAt = @UpdatedAt, IsActive = @IsActive WHERE Id = @Id;";
        return await connection.ExecuteAsync(sql, entity);
    }
}
