namespace BillingSystem.Infrastructure.Repositories;

using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Domain.Models;
using BillingSystem.Infrastructure.Data;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CouponRepository : ICouponRepository
{
    private readonly DbConnectionFactory _db;

    public CouponRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<Coupon?> GetByIdAsync(int id)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Coupon>(
            "SELECT * FROM coupons WHERE Id = @Id;", new { Id = id });
    }

    public async Task<Coupon?> GetByCodeAsync(string code)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Coupon>(
            "SELECT * FROM coupons WHERE Code = @Code;", new { Code = code });
    }

    public async Task<IEnumerable<Coupon>> GetAllAsync()
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<Coupon>("SELECT * FROM coupons ORDER BY CreatedAt DESC;");
    }

    public async Task<IEnumerable<Coupon>> GetAllActiveAsync()
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<Coupon>(
            "SELECT * FROM coupons WHERE IsActive = true ORDER BY CreatedAt DESC;");
    }

    public async Task<int> AddAsync(Coupon entity)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            INSERT INTO coupons (Code, DiscountPercentage, DiscountAmount, ValidFrom, ValidUntil, MaxUses, CurrentUses, IsActive, CreatedAt)
            VALUES (@Code, @DiscountPercentage, @DiscountAmount, @ValidFrom, @ValidUntil, @MaxUses, @CurrentUses, @IsActive, CURRENT_TIMESTAMP)
            RETURNING Id;
        ";
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task<int> UpdateAsync(Coupon entity)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            UPDATE coupons 
            SET Code = @Code, 
                DiscountPercentage = @DiscountPercentage, 
                DiscountAmount = @DiscountAmount, 
                ValidFrom = @ValidFrom, 
                ValidUntil = @ValidUntil, 
                MaxUses = @MaxUses, 
                CurrentUses = @CurrentUses, 
                IsActive = @IsActive
            WHERE Id = @Id;
        ";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var connection = _db.CreateConnection();
        return await connection.ExecuteAsync("DELETE FROM coupons WHERE Id = @Id;", new { Id = id });
    }

    public Task<PagedResult<Coupon>> GetPagedAsync(string search, int page, int pageSize)
    {
        throw new System.NotImplementedException();
    }
}
