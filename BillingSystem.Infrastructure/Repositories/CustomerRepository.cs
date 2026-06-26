using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public interface ICustomerRepository : IRepository<Customer> {}

public class CustomerRepository : ICustomerRepository
{
    private readonly DbConnectionFactory _conn;
    public CustomerRepository(DbConnectionFactory conn) => _conn = conn;

    public async Task<Customer?> GetByIdAsync(int id) {
        using var db = _conn.CreateConnection();
        return await db.QueryFirstOrDefaultAsync<Customer>("SELECT * FROM Customers WHERE Id = @Id AND IsActive = TRUE", new { Id = id });
    }
    public async Task<IEnumerable<Customer>> GetAllAsync() {
        using var db = _conn.CreateConnection();
        return await db.QueryAsync<Customer>("SELECT * FROM Customers WHERE IsActive = TRUE");
    }
    public async Task<int> AddAsync(Customer entity) {
        using var db = _conn.CreateConnection();
        var sql = "INSERT INTO Customers (Name, DocumentNumber, Email, Phone, Address) VALUES (@Name, @DocumentNumber, @Email, @Phone, @Address) RETURNING Id;";
        return await db.ExecuteScalarAsync<int>(sql, entity);
    }
    public async Task<int> UpdateAsync(Customer entity) {
        using var db = _conn.CreateConnection();
        var sql = "UPDATE Customers SET Name=@Name, DocumentNumber=@DocumentNumber, Email=@Email, Phone=@Phone, Address=@Address, UpdatedAt=CURRENT_TIMESTAMP WHERE Id=@Id;";
        return await db.ExecuteAsync(sql, entity);
    }
}
