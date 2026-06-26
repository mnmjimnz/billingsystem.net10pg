using BillingSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportRepository _repo;
    public ReportsController(IReportRepository repo) => _repo = repo;

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var stats = await _repo.GetDashboardStatsAsync();
        var topProducts = await _repo.GetTopProductsAsync();
        return Ok(new { stats, topProducts });
    }
}
