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

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] string search = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(await _receivableService.GetPagedAsync(search, page, pageSize));
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

    [HttpGet("{id}/payments")]
    public async Task<IActionResult> GetPayments(int id)
    {
        var payments = await _receivableService.GetPaymentsAsync(id);
        return Ok(payments);
    }
}
