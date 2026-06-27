using BillingSystem.Application.DTOs;
using BillingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("sales")]
    public async Task<IActionResult> GetSales(
        [FromQuery] DateTime? startDate, 
        [FromQuery] DateTime? endDate, 
        [FromQuery] int? branchId, 
        [FromQuery] int? userId, 
        [FromQuery] string? paymentType)
    {
        var filter = new ReportFilterDto
        {
            StartDate = startDate,
            EndDate = endDate,
            BranchId = branchId,
            UserId = userId,
            PaymentType = paymentType
        };
        var result = await _reportService.GetSalesReportAsync(filter);
        return Ok(result);
    }

    [HttpGet("top-products")]
    public async Task<IActionResult> GetTopProducts([FromQuery] int limit = 10)
    {
        var result = await _reportService.GetTopProductsAsync(limit);
        return Ok(result);
    }

    [HttpGet("top-suppliers")]
    public async Task<IActionResult> GetTopSuppliers([FromQuery] int limit = 10)
    {
        var result = await _reportService.GetTopSuppliersAsync(limit);
        return Ok(result);
    }

    [HttpGet("cash-flow")]
    public async Task<IActionResult> GetCashFlow(
        [FromQuery] DateTime? startDate, 
        [FromQuery] DateTime? endDate, 
        [FromQuery] int? branchId)
    {
        var filter = new ReportFilterDto
        {
            StartDate = startDate,
            EndDate = endDate,
            BranchId = branchId
        };
        var result = await _reportService.GetCashFlowAsync(filter);
        return Ok(result);
    }

    [HttpGet("kardex")]
    public async Task<IActionResult> GetKardex(
        [FromQuery] DateTime? startDate, 
        [FromQuery] DateTime? endDate)
    {
        var filter = new ReportFilterDto
        {
            StartDate = startDate,
            EndDate = endDate
        };
        var result = await _reportService.GetKardexReportAsync(filter);
        return Ok(result);
    }

    [HttpGet("paged-kardex")]
    public async Task<IActionResult> GetPagedKardex(
        [FromQuery] DateTime? startDate, 
        [FromQuery] DateTime? endDate,
        [FromQuery] string search = "",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var filter = new ReportFilterDto
        {
            StartDate = startDate,
            EndDate = endDate
        };
        var result = await _reportService.GetPagedKardexAsync(filter, search, page, pageSize);
        return Ok(result);
    }

    [HttpGet("sales-comparison")]
    public async Task<IActionResult> GetSalesComparison(
        [FromQuery] string periodType = "day",
        [FromQuery] DateTime? startDate = null, 
        [FromQuery] DateTime? endDate = null, 
        [FromQuery] int? branchId = null)
    {
        var filter = new ReportFilterDto
        {
            StartDate = startDate,
            EndDate = endDate,
            BranchId = branchId
        };
        var result = await _reportService.GetSalesComparisonAsync(periodType, filter);
        return Ok(result);
    }
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var result = await _reportService.GetDashboardDataAsync();
        return Ok(result);
    }

    [HttpGet("user-activity")]
    public async Task<IActionResult> GetUserActivity([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int? userId, [FromQuery] int? branchId)
    {
        var filter = new ReportFilterDto { StartDate = startDate, EndDate = endDate, UserId = userId, BranchId = branchId };
        return Ok(await _reportService.GetUserActivityAsync(filter));
    }

    [HttpGet("balance-sheet")]
    public async Task<IActionResult> GetBalanceSheet()
    {
        return Ok(await _reportService.GetBalanceSheetAsync());
    }

    [HttpGet("income-statement")]
    public async Task<IActionResult> GetIncomeStatement([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var filter = new ReportFilterDto { StartDate = startDate, EndDate = endDate };
        return Ok(await _reportService.GetIncomeStatementAsync(filter));
    }

    [HttpGet("purchases")]
    public async Task<IActionResult> GetPurchases([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int? branchId)
    {
        var filter = new ReportFilterDto { StartDate = startDate, EndDate = endDate, BranchId = branchId };
        return Ok(await _reportService.GetPurchasesReportAsync(filter));
    }

    [HttpGet("sales-analytics")]
    public async Task<IActionResult> GetSalesAnalytics([FromQuery] string groupBy = "month", [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var filter = new ReportFilterDto { StartDate = startDate, EndDate = endDate };
        return Ok(await _reportService.GetSalesAnalyticsAsync(groupBy, filter));
    }
}