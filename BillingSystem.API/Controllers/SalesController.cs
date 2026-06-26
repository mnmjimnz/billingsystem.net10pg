using BillingSystem.Application.DTOs;
using BillingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SalesController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request)
    {
        int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out int userId);
        int.TryParse(User.FindFirst("BranchId")?.Value, out int branchId);

        var result = await _saleService.CreateSaleAsync(request, userId, branchId);
        
        return Ok(new { message = "Venta procesada exitosamente", saleId = result.SaleId, ticketNumber = result.TicketNumber });
    }
}
