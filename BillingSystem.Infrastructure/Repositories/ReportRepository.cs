using BillingSystem.Application.DTOs;
using BillingSystem.Application.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly DbConnectionFactory _db;

    public ReportRepository(DbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<SalesReportDto>> GetSalesReportAsync(ReportFilterDto filter)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            SELECT 
                s.Id as SaleId,
                s.TicketNumber,
                s.CreatedAt,
                c.Name as CustomerName,
                u.FullName as UserName,
                b.Name as BranchName,
                s.Total,
                s.PaymentType
            FROM Sales s
            LEFT JOIN Customers c ON s.CustomerId = c.Id
            LEFT JOIN Users u ON s.UserId = u.Id
            LEFT JOIN Branches b ON s.BranchId = b.Id
            WHERE 1=1 ";

        if (filter.StartDate.HasValue) sql += " AND s.CreatedAt >= @StartDate ";
        if (filter.EndDate.HasValue) sql += " AND s.CreatedAt <= @EndDate ";
        if (filter.BranchId.HasValue && filter.BranchId > 0) sql += " AND s.BranchId = @BranchId ";
        if (filter.UserId.HasValue && filter.UserId > 0) sql += " AND s.UserId = @UserId ";
        if (!string.IsNullOrEmpty(filter.PaymentType)) sql += " AND s.PaymentType = @PaymentType ";

        sql += " ORDER BY s.CreatedAt DESC";

        return await connection.QueryAsync<SalesReportDto>(sql, filter);
    }

    public async Task<IEnumerable<TopProductDto>> GetTopProductsAsync(int limit)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            SELECT 
                p.Id as ProductId,
                p.Name as ProductName,
                p.Barcode,
                SUM(sd.Quantity) as TotalQuantitySold,
                SUM(sd.Subtotal) as TotalRevenue
            FROM PurchaseDetails pd -- Wait, SaleDetails
            ";
            
        sql = @"
            SELECT 
                p.Id as ProductId,
                p.Name as ProductName,
                p.Barcode,
                CAST(COALESCE(SUM(sd.Quantity), 0) AS INT) as TotalQuantitySold,
                COALESCE(SUM(sd.Subtotal), 0) as TotalRevenue
            FROM SaleDetails sd
            JOIN Products p ON sd.ProductId = p.Id
            GROUP BY p.Id, p.Name, p.Barcode
            ORDER BY TotalQuantitySold DESC
            LIMIT @Limit";

        return await connection.QueryAsync<TopProductDto>(sql, new { Limit = limit });
    }

    public async Task<IEnumerable<TopSupplierDto>> GetTopSuppliersAsync(int limit)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            SELECT 
                s.Id as SupplierId,
                s.Name as SupplierName,
                CAST(COUNT(p.Id) AS INT) as TotalPurchases,
                COALESCE(SUM(p.Total), 0) as TotalVolume
            FROM Purchases p
            JOIN Suppliers s ON p.SupplierId = s.Id
            GROUP BY s.Id, s.Name
            ORDER BY TotalVolume DESC
            LIMIT @Limit";

        return await connection.QueryAsync<TopSupplierDto>(sql, new { Limit = limit });
    }

    public async Task<IEnumerable<CashFlowDto>> GetCashFlowAsync(ReportFilterDto filter)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            WITH CashEntries AS (
                -- Ventas al contado
                SELECT 
                    s.CreatedAt as Date, 
                    'Venta al Contado ' || s.TicketNumber as Description, 
                    'IN' as Type, 
                    'SALE' as Category, 
                    s.Total as Amount,
                    s.BranchId
                FROM Sales s
                WHERE s.PaymentType = 'CASH'

                UNION ALL

                -- Abonos a cuentas por cobrar
                SELECT 
                    p.CreatedAt as Date, 
                    'Abono Cliente (' || p.Notes || ')' as Description, 
                    'IN' as Type, 
                    'AR_PAYMENT' as Category, 
                    p.Amount,
                    s.BranchId
                FROM ReceivablePayments p
                JOIN AccountsReceivable ar ON p.AccountId = ar.Id
                JOIN Sales s ON ar.SaleId = s.Id

                UNION ALL

                -- Compras al contado
                SELECT 
                    p.CreatedAt as Date, 
                    'Compra al Contado ' || p.InvoiceNumber as Description, 
                    'OUT' as Type, 
                    'PURCHASE' as Category, 
                    p.Total as Amount,
                    NULL as BranchId
                FROM Purchases p
                WHERE p.PaymentType = 'CASH'

                UNION ALL

                -- Abonos a proveedores
                SELECT 
                    pp.CreatedAt as Date, 
                    'Abono Proveedor (' || pp.Notes || ')' as Description, 
                    'OUT' as Type, 
                    'AP_PAYMENT' as Category, 
                    pp.Amount,
                    NULL as BranchId
                FROM PayablePayments pp
                
                UNION ALL
                
                -- Movimientos Financieros Manuales (Inyecciones, Planillas, Servicios, etc.)
                SELECT 
                    bm.Date as Date,
                    bm.Category || COALESCE(' - ' || bm.Description, '') as Description,
                    bm.Type as Type,
                    'BRANCH_MOVEMENT' as Category,
                    bm.Amount as Amount,
                    bm.BranchId as BranchId
                FROM BranchMovements bm
                WHERE bm.IsActive = TRUE
            )
            SELECT 
                ce.*,
                b.Name as BranchName
            FROM CashEntries ce
            LEFT JOIN Branches b ON ce.BranchId = b.Id
            WHERE 1=1 ";

        if (filter.StartDate.HasValue) sql += " AND ce.Date >= @StartDate ";
        if (filter.EndDate.HasValue) sql += " AND ce.Date <= @EndDate ";
        if (filter.BranchId.HasValue && filter.BranchId > 0) sql += " AND (ce.BranchId = @BranchId OR ce.BranchId IS NULL) ";

        sql += " ORDER BY ce.Date DESC";

        return await connection.QueryAsync<CashFlowDto>(sql, filter);
    }

    public async Task<IEnumerable<KardexReportDto>> GetKardexReportAsync(ReportFilterDto filter)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            SELECT 
                im.Id,
                p.Name as ProductName,
                im.MovementType,
                im.ReferenceType,
                im.ReferenceId,
                im.Quantity,
                im.PreviousStock,
                im.NewStock,
                im.Description,
                im.CreatedAt
            FROM InventoryMovements im
            JOIN Products p ON im.ProductId = p.Id
            WHERE 1=1 ";

        if (filter.StartDate.HasValue) sql += " AND im.CreatedAt >= @StartDate ";
        if (filter.EndDate.HasValue) sql += " AND im.CreatedAt <= @EndDate ";

        sql += " ORDER BY im.CreatedAt DESC";

        return await connection.QueryAsync<KardexReportDto>(sql, filter);
    }

    public async Task<BillingSystem.Domain.Models.PagedResult<KardexReportDto>> GetPagedKardexAsync(ReportFilterDto filter, string search, int page, int pageSize)
    {
        using var connection = _db.CreateConnection();
        var searchPattern = $"%{search}%";
        var offset = (page - 1) * pageSize;
        var limit = pageSize > 0 ? pageSize : 10;
        
        var baseSql = @"FROM InventoryMovements im JOIN Products p ON im.ProductId = p.Id WHERE 1=1 ";
        
        if (filter.StartDate.HasValue) baseSql += " AND im.CreatedAt >= @StartDate ";
        if (filter.EndDate.HasValue) baseSql += " AND im.CreatedAt <= @EndDate ";
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            baseSql += " AND (im.Description ILIKE @Search OR p.Name ILIKE @Search OR im.MovementType ILIKE @Search OR im.ReferenceType ILIKE @Search)";
        }
        
        var countSql = $"SELECT COUNT(*) {baseSql}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { Search = searchPattern, StartDate = filter.StartDate, EndDate = filter.EndDate });
        
        var dataSql = $"SELECT im.Id, p.Name as ProductName, im.MovementType, im.ReferenceType, im.ReferenceId, im.Quantity, im.PreviousStock, im.NewStock, im.Description, im.CreatedAt {baseSql} ORDER BY im.CreatedAt DESC LIMIT @Limit OFFSET @Offset";
        var items = await connection.QueryAsync<KardexReportDto>(dataSql, new { Search = searchPattern, StartDate = filter.StartDate, EndDate = filter.EndDate, Limit = limit, Offset = offset });
        
        return new BillingSystem.Domain.Models.PagedResult<KardexReportDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<IEnumerable<SalesComparisonDto>> GetSalesComparisonAsync(string periodType, ReportFilterDto filter)
    {
        using var connection = _db.CreateConnection();
        string periodSelector;
        
        switch (periodType?.ToLower())
        {
            case "month":
                periodSelector = "TO_CHAR(CreatedAt, 'YYYY-MM')";
                break;
            case "year":
                periodSelector = "TO_CHAR(CreatedAt, 'YYYY')";
                break;
            case "dayofweek":
                periodSelector = "TO_CHAR(CreatedAt, 'Day')";
                break;
            default: // day
                periodSelector = "TO_CHAR(CreatedAt, 'YYYY-MM-DD')";
                break;
        }

        var sql = $@"
            SELECT 
                {periodSelector} as Period,
                SUM(Total) as TotalSales,
                COUNT(Id) as SaleCount
            FROM Sales
            WHERE 1=1 ";

        if (filter.StartDate.HasValue) sql += " AND CreatedAt >= @StartDate ";
        if (filter.EndDate.HasValue) sql += " AND CreatedAt <= @EndDate ";
        if (filter.BranchId.HasValue && filter.BranchId > 0) sql += " AND BranchId = @BranchId ";

        sql += $@" GROUP BY {periodSelector} ORDER BY Period DESC";

        return await connection.QueryAsync<SalesComparisonDto>(sql, filter);
    }
    public async Task<DashboardDataDto> GetDashboardDataAsync()
    {
        using var connection = _db.CreateConnection();
        var today = DateTime.Today;

        var stats = new DashboardStatsDto();

        stats.TodaySales = await connection.ExecuteScalarAsync<decimal>("SELECT COALESCE(SUM(Total), 0) FROM Sales WHERE DATE(CreatedAt) = CURRENT_DATE");
        stats.TodayPurchases = await connection.ExecuteScalarAsync<decimal>("SELECT COALESCE(SUM(Total), 0) FROM Purchases WHERE DATE(CreatedAt) = CURRENT_DATE");
        stats.TotalProducts = await connection.ExecuteScalarAsync<int>("SELECT CAST(COUNT(*) AS INT) FROM Products WHERE IsActive = TRUE");
        stats.TotalCustomers = await connection.ExecuteScalarAsync<int>("SELECT CAST(COUNT(*) AS INT) FROM Customers WHERE IsActive = TRUE");

        var topProducts = await GetTopProductsAsync(5);

        return new DashboardDataDto
        {
            Stats = stats,
            TopProducts = topProducts
        };
    }

    public async Task<IEnumerable<UserActivityDto>> GetUserActivityAsync(ReportFilterDto filter)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            SELECT 
                crs.Id as SessionId,
                u.FullName as UserName,
                b.Name as BranchName,
                crs.OpeningTime,
                crs.ClosingTime,
                crs.OpeningBalance,
                crs.ClosingBalance,
                crs.Status
            FROM CashRegisterSessions crs
            JOIN Users u ON crs.UserId = u.Id
            JOIN CashRegisters cr ON crs.CashRegisterId = cr.Id
            JOIN Branches b ON cr.BranchId = b.Id
            WHERE 1=1 ";

        if (filter.StartDate.HasValue) sql += " AND crs.OpeningTime >= @StartDate ";
        if (filter.EndDate.HasValue) sql += " AND crs.OpeningTime <= @EndDate ";
        if (filter.UserId.HasValue && filter.UserId > 0) sql += " AND crs.UserId = @UserId ";
        if (filter.BranchId.HasValue && filter.BranchId > 0) sql += " AND cr.BranchId = @BranchId ";

        sql += " ORDER BY crs.OpeningTime DESC";

        return await connection.QueryAsync<UserActivityDto>(sql, filter);
    }

    public async Task<BalanceSheetDto> GetBalanceSheetAsync()
    {
        using var connection = _db.CreateConnection();
        
        var cashBranches = await connection.ExecuteScalarAsync<decimal>("SELECT COALESCE(SUM(AvailableFunds), 0) FROM Branches WHERE IsActive = TRUE");
        var cashSessions = await connection.ExecuteScalarAsync<decimal>("SELECT COALESCE(SUM(OpeningBalance), 0) FROM CashRegisterSessions WHERE Status = 'OPEN'"); // Approximate active cash in registers not yet closed to branch
        
        var inventoryValue = await connection.ExecuteScalarAsync<decimal>("SELECT COALESCE(SUM(Stock * Cost), 0) FROM Products WHERE IsActive = TRUE");
        
        var accountsReceivable = await connection.ExecuteScalarAsync<decimal>("SELECT COALESCE(SUM(Balance), 0) FROM AccountsReceivable");
        
        var accountsPayable = await connection.ExecuteScalarAsync<decimal>("SELECT COALESCE(SUM(Balance), 0) FROM AccountsPayable");

        return new BalanceSheetDto
        {
            CashAndEquivalents = cashBranches + cashSessions,
            InventoryValue = inventoryValue,
            AccountsReceivable = accountsReceivable,
            AccountsPayable = accountsPayable
        };
    }

    public async Task<IncomeStatementDto> GetIncomeStatementAsync(ReportFilterDto filter)
    {
        using var connection = _db.CreateConnection();
        var filterSqlSales = " WHERE 1=1 ";
        if (filter.StartDate.HasValue) filterSqlSales += " AND CreatedAt >= @StartDate ";
        if (filter.EndDate.HasValue) filterSqlSales += " AND CreatedAt <= @EndDate ";

        var filterSqlMoves = " WHERE IsActive = TRUE AND Type = 'OUT' ";
        if (filter.StartDate.HasValue) filterSqlMoves += " AND Date >= @StartDate ";
        if (filter.EndDate.HasValue) filterSqlMoves += " AND Date <= @EndDate ";

        var revenue = await connection.ExecuteScalarAsync<decimal>($"SELECT COALESCE(SUM(Total), 0) FROM Sales {filterSqlSales}", filter);
        
        var cogsSql = $@"
            SELECT COALESCE(SUM(sd.Quantity * p.Cost), 0)
            FROM SaleDetails sd
            JOIN Sales s ON sd.SaleId = s.Id
            JOIN Products p ON sd.ProductId = p.Id
            {filterSqlSales.Replace("CreatedAt", "s.CreatedAt")}";
        var cogs = await connection.ExecuteScalarAsync<decimal>(cogsSql, filter);

        var expenses = await connection.ExecuteScalarAsync<decimal>($"SELECT COALESCE(SUM(Amount), 0) FROM BranchMovements {filterSqlMoves}", filter);

        return new IncomeStatementDto
        {
            Revenue = revenue,
            CostOfGoodsSold = cogs,
            OperatingExpenses = expenses
        };
    }

    public async Task<IEnumerable<PurchaseReportDto>> GetPurchasesReportAsync(ReportFilterDto filter)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            SELECT 
                p.Id as PurchaseId,
                p.InvoiceNumber,
                p.CreatedAt,
                s.Name as SupplierName,
                b.Name as BranchName,
                u.FullName as UserName,
                p.Total,
                p.PaymentType,
                p.Status
            FROM Purchases p
            JOIN Suppliers s ON p.SupplierId = s.Id
            JOIN Users u ON p.UserId = u.Id
            LEFT JOIN Branches b ON p.BranchId = b.Id
            WHERE 1=1 ";

        if (filter.StartDate.HasValue) sql += " AND p.CreatedAt >= @StartDate ";
        if (filter.EndDate.HasValue) sql += " AND p.CreatedAt <= @EndDate ";
        if (filter.BranchId.HasValue && filter.BranchId > 0) sql += " AND p.BranchId = @BranchId ";

        sql += " ORDER BY p.CreatedAt DESC";

        return await connection.QueryAsync<PurchaseReportDto>(sql, filter);
    }

    public async Task<IEnumerable<SalesAnalyticsDto>> GetSalesAnalyticsAsync(string groupBy, ReportFilterDto filter)
    {
        using var connection = _db.CreateConnection();
        string periodSelector = groupBy?.ToLower() == "month" ? "TO_CHAR(CreatedAt, 'YYYY-MM')" : "TO_CHAR(CreatedAt, 'YYYY-MM-DD')";

        var sql = $@"
            SELECT 
                {periodSelector} as GroupKey,
                COALESCE(SUM(CASE WHEN PaymentType = 'CASH' THEN Total ELSE 0 END), 0) as CashSales,
                COALESCE(SUM(CASE WHEN PaymentType = 'CREDIT' THEN Total ELSE 0 END), 0) as CreditSales
            FROM Sales
            WHERE 1=1 ";

        if (filter.StartDate.HasValue) sql += " AND CreatedAt >= @StartDate ";
        if (filter.EndDate.HasValue) sql += " AND CreatedAt <= @EndDate ";

        sql += $@" GROUP BY {periodSelector} ORDER BY GroupKey ASC";

        return await connection.QueryAsync<SalesAnalyticsDto>(sql, filter);
    }
}