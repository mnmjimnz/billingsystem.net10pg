using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class DashboardStatsDto
{
    public int TotalProducts { get; set; }
    public int TotalCustomers { get; set; }
    public decimal TodaySales { get; set; }
    public decimal TodayPurchases { get; set; }
}

public class TopProductDto
{
    public string Name { get; set; }
    public int TotalSold { get; set; }
}

public interface IReportRepository 
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<IEnumerable<TopProductDto>> GetTopProductsAsync();
}

public class ReportRepository : IReportRepository
{
    private readonly DbConnectionFactory _conn;
    public ReportRepository(DbConnectionFactory conn) => _conn = conn;

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        using var db = _conn.CreateConnection();
        var sql = @"
            SELECT 
                (SELECT COUNT(*) FROM Products WHERE IsActive = TRUE) as TotalProducts,
                (SELECT COUNT(*) FROM Customers WHERE IsActive = TRUE) as TotalCustomers,
                (SELECT COALESCE(SUM(Total), 0) FROM Sales WHERE Date::date = CURRENT_DATE AND IsActive = TRUE) as TodaySales,
                (SELECT COALESCE(SUM(Total), 0) FROM Purchases WHERE Date::date = CURRENT_DATE AND IsActive = TRUE) as TodayPurchases;
        ";
        return await db.QueryFirstOrDefaultAsync<DashboardStatsDto>(sql);
    }

    public async Task<IEnumerable<TopProductDto>> GetTopProductsAsync()
    {
        using var db = _conn.CreateConnection();
        var sql = @"
            SELECT p.Name, SUM(sd.Quantity) as TotalSold
            FROM SaleDetails sd
            JOIN Products p ON p.Id = sd.ProductId
            GROUP BY p.Id, p.Name
            ORDER BY TotalSold DESC
            LIMIT 5;
        ";
        return await db.QueryAsync<TopProductDto>(sql);
    }
}
