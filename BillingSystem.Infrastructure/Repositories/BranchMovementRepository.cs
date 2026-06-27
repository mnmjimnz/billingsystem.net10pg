using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class BranchMovementRepository : IBranchMovementRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public BranchMovementRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<BranchMovement?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<BranchMovement>("SELECT * FROM BranchMovements WHERE Id = @Id AND IsActive = TRUE", new { Id = id });
    }

    public async Task<IEnumerable<BranchMovement>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<BranchMovement>("SELECT * FROM BranchMovements WHERE IsActive = TRUE ORDER BY Date DESC");
    }

    public async Task<int> AddAsync(BranchMovement entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"INSERT INTO BranchMovements (BranchId, Amount, Type, Category, Description, UserId, EmployeeId, Date, CreatedAt, IsActive) 
                    VALUES (@BranchId, @Amount, @Type, @Category, @Description, @UserId, @EmployeeId, @Date, @CreatedAt, @IsActive) RETURNING Id;";
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task<int> UpdateAsync(BranchMovement entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"UPDATE BranchMovements SET Amount = @Amount, Type = @Type, Category = @Category, Description = @Description, EmployeeId = @EmployeeId, Date = @Date, UpdatedAt = @UpdatedAt, IsActive = @IsActive WHERE Id = @Id;";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<IEnumerable<BranchMovement>> GetByBranchIdAsync(int branchId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"SELECT m.*, u.FullName as UserName, e.FullName as EmployeeName 
                    FROM BranchMovements m 
                    LEFT JOIN Users u ON m.UserId = u.Id 
                    LEFT JOIN Users e ON m.EmployeeId = e.Id 
                    WHERE m.BranchId = @BranchId AND m.IsActive = TRUE 
                    ORDER BY m.Date DESC";
        // Need mapping for joins or just return basic
        return await connection.QueryAsync<BranchMovement>(sql, new { BranchId = branchId });
    }

    public async Task<BillingSystem.Domain.Models.PagedResult<BranchMovement>> GetPagedAsync(string search, int page, int pageSize)
    {
        using var connection = _connectionFactory.CreateConnection();
        var searchPattern = $"%{search}%";
        var offset = (page - 1) * pageSize;
        var limit = pageSize > 0 ? pageSize : 10;
        
        var baseSql = "FROM BranchMovements WHERE IsActive = TRUE";
        if (!string.IsNullOrWhiteSpace(search))
        {
            baseSql += " AND (Category ILIKE @Search OR Description ILIKE @Search)";
        }
        
        var countSql = $"SELECT COUNT(*) {baseSql}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { Search = searchPattern });
        
        var dataSql = $"SELECT * {baseSql} ORDER BY Date DESC LIMIT @Limit OFFSET @Offset";
        var items = await connection.QueryAsync<BranchMovement>(dataSql, new { Search = searchPattern, Limit = limit, Offset = offset });
        
        return new BillingSystem.Domain.Models.PagedResult<BranchMovement>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
