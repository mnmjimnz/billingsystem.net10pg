using BillingSystem.Application.DTOs;

namespace BillingSystem.Application.Interfaces;

public interface IReportRepository
{
    Task<IEnumerable<SalesReportDto>> GetSalesReportAsync(ReportFilterDto filter);
    Task<IEnumerable<TopProductDto>> GetTopProductsAsync(int limit);
    Task<IEnumerable<TopSupplierDto>> GetTopSuppliersAsync(int limit);
    Task<IEnumerable<CashFlowDto>> GetCashFlowAsync(ReportFilterDto filter);
    Task<IEnumerable<KardexReportDto>> GetKardexReportAsync(ReportFilterDto filter);
    Task<IEnumerable<SalesComparisonDto>> GetSalesComparisonAsync(string periodType, ReportFilterDto filter);
    Task<DashboardDataDto> GetDashboardDataAsync();
}
