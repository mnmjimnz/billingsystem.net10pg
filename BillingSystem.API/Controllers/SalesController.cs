using BillingSystem.Application.DTOs;
using BillingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BillingSystem.API.Extensions;

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
        try
        {
            int.TryParse(User.FindFirst("UserId")?.Value, out int userId);
            
            int branchId;
            if (User.IsAdmin() && request.BranchId.HasValue && request.BranchId.Value > 0)
            {
                branchId = request.BranchId.Value;
            }
            else
            {
                int.TryParse(User.FindFirst("BranchId")?.Value, out branchId);
            }

            var result = await _saleService.CreateSaleAsync(request, userId, branchId);
            
            return Ok(new { success = true, message = "Venta procesada exitosamente", saleId = result.SaleId, ticketNumber = result.TicketNumber });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
