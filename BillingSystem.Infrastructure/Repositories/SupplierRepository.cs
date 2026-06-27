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

    public async Task<int> UpdateAsync(Supplier entity) {
        using var db = _connectionFactory.CreateConnection();
        var sql = "UPDATE Suppliers SET Name=@Name, DocumentNumber=@DocumentNumber, Email=@Email, Phone=@Phone, Address=@Address, UpdatedAt=CURRENT_TIMESTAMP WHERE Id=@Id;";
        return await db.ExecuteAsync(sql, entity);
    }

    public async Task<BillingSystem.Domain.Models.PagedResult<Supplier>> GetPagedAsync(string search, int page, int pageSize)
    {
        using var db = _connectionFactory.CreateConnection();
        var searchPattern = $"%{search}%";
        var offset = (page - 1) * pageSize;
        var limit = pageSize > 0 ? pageSize : 10;
        
        var baseSql = "FROM Suppliers WHERE IsActive = TRUE";
        if (!string.IsNullOrWhiteSpace(search))
        {
            baseSql += " AND (Name ILIKE @Search OR DocumentNumber ILIKE @Search OR Email ILIKE @Search OR Phone ILIKE @Search OR Address ILIKE @Search)";
        }
        
        var countSql = $"SELECT COUNT(*) {baseSql}";
        var totalCount = await db.ExecuteScalarAsync<int>(countSql, new { Search = searchPattern });
        
        var dataSql = $"SELECT * {baseSql} ORDER BY Id DESC LIMIT @Limit OFFSET @Offset";
        var items = await db.QueryAsync<Supplier>(dataSql, new { Search = searchPattern, Limit = limit, Offset = offset });
        
        return new BillingSystem.Domain.Models.PagedResult<Supplier>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
