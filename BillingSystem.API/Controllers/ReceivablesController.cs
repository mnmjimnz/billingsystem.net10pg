using BillingSystem.Application.DTOs;
using BillingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReceivablesController : ControllerBase
{
    private readonly IReceivableService _receivableService;

    public ReceivablesController(IReceivableService receivableService)
    {
        _receivableService = receivableService;
    }

    [HttpGet]
    public async Task<IActionResult> GetReceivables()
    {
        var result = await _receivableService.GetReceivablesAsync();
        return Ok(result);
    }

    [HttpPost("{id}/pay")]
    public async Task<IActionResult> RegisterPayment(int id, [FromBody] PaymentRequest request)
    {
        try
        {
            int.TryParse(User.FindFirst("UserId")?.Value, out int userId);
            await _receivableService.RegisterPaymentAsync(id, userId == 0 ? 1 : userId, request.Amount, request.Notes ?? string.Empty);
            return Ok(new { message = "Abono registrado correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
