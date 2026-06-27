using BillingSystem.Application.DTOs;
using BillingSystem.Application.Interfaces;
using BillingSystem.Domain.Interfaces;

namespace BillingSystem.Application.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _repo;

    public ReportService(IReportRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<SalesReportDto>> GetSalesReportAsync(ReportFilterDto filter)
    {
        return await _repo.GetSalesReportAsync(filter);
    }

    public async Task<IEnumerable<TopProductDto>> GetTopProductsAsync(int limit)
    {
        return await _repo.GetTopProductsAsync(limit);
    }

    public async Task<IEnumerable<TopSupplierDto>> GetTopSuppliersAsync(int limit)
    {
        return await _repo.GetTopSuppliersAsync(limit);
    }

    public async Task<IEnumerable<CashFlowDto>> GetCashFlowAsync(ReportFilterDto filter)
    {
        return await _repo.GetCashFlowAsync(filter);
    }

    public async Task<IEnumerable<KardexReportDto>> GetKardexReportAsync(ReportFilterDto filter)
    {
        return await _repo.GetKardexReportAsync(filter);
    }

    public async Task<IEnumerable<SalesComparisonDto>> GetSalesComparisonAsync(string periodType, ReportFilterDto filter)
    {
        return await _repo.GetSalesComparisonAsync(periodType, filter);
    }
    public async Task<DashboardDataDto> GetDashboardDataAsync()
    {
        return await _repo.GetDashboardDataAsync();
    }
    public async Task<BillingSystem.Domain.Models.PagedResult<KardexReportDto>> GetPagedKardexAsync(ReportFilterDto filter, string search, int page, int pageSize)
    {
        return await _repo.GetPagedKardexAsync(filter, search, page, pageSize);
    }
    public async Task<IEnumerable<UserActivityDto>> GetUserActivityAsync(ReportFilterDto filter)
    {
        return await _repo.GetUserActivityAsync(filter);
    }
    public async Task<BalanceSheetDto> GetBalanceSheetAsync()
    {
        return await _repo.GetBalanceSheetAsync();
    }
    public async Task<IncomeStatementDto> GetIncomeStatementAsync(ReportFilterDto filter)
    {
        return await _repo.GetIncomeStatementAsync(filter);
    }
    public async Task<IEnumerable<PurchaseReportDto>> GetPurchasesReportAsync(ReportFilterDto filter)
    {
        return await _repo.GetPurchasesReportAsync(filter);
    }
    public async Task<IEnumerable<SalesAnalyticsDto>> GetSalesAnalyticsAsync(string groupBy, ReportFilterDto filter)
    {
        return await _repo.GetSalesAnalyticsAsync(groupBy, filter);
    }
}