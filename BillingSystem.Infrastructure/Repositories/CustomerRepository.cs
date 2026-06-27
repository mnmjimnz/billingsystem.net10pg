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

    public async Task<BillingSystem.Domain.Models.PagedResult<Customer>> GetPagedAsync(string search, int page, int pageSize)
    {
        using var db = _conn.CreateConnection();
        var searchPattern = $"%{search}%";
        var offset = (page - 1) * pageSize;
        var limit = pageSize > 0 ? pageSize : 10;
        
        var baseSql = "FROM Customers WHERE IsActive = TRUE";
        if (!string.IsNullOrWhiteSpace(search))
        {
            baseSql += " AND (Name ILIKE @Search OR DocumentNumber ILIKE @Search OR Email ILIKE @Search OR Phone ILIKE @Search OR Address ILIKE @Search)";
        }
        
        var countSql = $"SELECT COUNT(*) {baseSql}";
        var totalCount = await db.ExecuteScalarAsync<int>(countSql, new { Search = searchPattern });
        
        var dataSql = $"SELECT * {baseSql} ORDER BY Id DESC LIMIT @Limit OFFSET @Offset";
        var items = await db.QueryAsync<Customer>(dataSql, new { Search = searchPattern, Limit = limit, Offset = offset });
        
        return new BillingSystem.Domain.Models.PagedResult<Customer>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
