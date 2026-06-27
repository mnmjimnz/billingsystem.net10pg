using System.Data;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly DbConnectionFactory _db;
    public SaleRepository(DbConnectionFactory db) => _db = db;

    public Task<Sale?> GetByIdAsync(int id) => throw new NotImplementedException();
    public Task<IEnumerable<Sale>> GetAllAsync() => throw new NotImplementedException();
    public Task<int> AddAsync(Sale entity) => throw new NotImplementedException();
    public Task<int> UpdateAsync(Sale entity) => throw new NotImplementedException();

    public Task<BillingSystem.Domain.Models.PagedResult<Sale>> GetPagedAsync(string search, int page, int pageSize)
    {
        // Not implemented since Sale pagination wasn't explicitly requested, but required by interface
        return Task.FromResult(new BillingSystem.Domain.Models.PagedResult<Sale>());
    }

    public async Task<int> CreateSaleWithDetailsAsync(Sale sale, IEnumerable<SaleDetail> details)
    {
        using var connection = _db.CreateConnection();
        var saleSql = @"INSERT INTO Sales (TicketNumber, CustomerId, UserId, BranchId, Subtotal, Discount, TaxAmount, Total, PaymentType, AmountTendered, Change, Status) 
                        VALUES (@TicketNumber, @CustomerId, @UserId, @BranchId, @Subtotal, @Discount, @TaxAmount, @Total, @PaymentType, @AmountTendered, @Change, @Status) RETURNING Id;";
        var saleId = await connection.ExecuteScalarAsync<int>(saleSql, sale);

        foreach(var detail in details) {
            detail.SaleId = saleId;
            var detailSql = @"INSERT INTO SaleDetails (SaleId, ProductId, Quantity, UnitPrice, Subtotal) 
                              VALUES (@SaleId, @ProductId, @Quantity, @UnitPrice, @Subtotal);";
            await connection.ExecuteAsync(detailSql, detail);
        }

        return saleId;
    }

    public async Task<decimal> GetSessionSalesTotalAsync(int userId, DateTime since)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            SELECT 
                (SELECT COALESCE(SUM(Total), 0) FROM Sales WHERE UserId = @UserId AND Date >= @Since AND Status = 'PAID')
                +
                (SELECT COALESCE(SUM(Amount), 0) FROM ReceivablePayments WHERE UserId = @UserId AND PaymentDate >= @Since)
            AS TotalSessionCash;";
        return await connection.ExecuteScalarAsync<decimal>(sql, new { UserId = userId, Since = since });
    }
}
