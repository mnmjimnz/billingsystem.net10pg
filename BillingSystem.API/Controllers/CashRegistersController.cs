using BillingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CashRegistersController : ControllerBase
{
    private readonly ICashRegisterService _cashService;

    public CashRegistersController(ICashRegisterService cashService)
    {
        _cashService = cashService;
    }

    private int GetCurrentUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpGet("session")]
    public async Task<IActionResult> GetActiveSession()
    {
        var session = await _cashService.GetActiveSessionAsync(GetCurrentUserId());
        return Ok(new { success = true, hasOpenSession = session != null, session });
    }

    [HttpPost("open")]
    public async Task<IActionResult> OpenSession([FromBody] OpenSessionRequest req)
    {
        try
        {
            var id = await _cashService.OpenSessionAsync(GetCurrentUserId(), req.BranchId, req.OpeningBalance);
            return Ok(new { success = true, sessionId = id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("close")]
    public async Task<IActionResult> CloseSession([FromBody] CloseSessionRequest req)
    {
        try
        {
            await _cashService.CloseSessionAsync(GetCurrentUserId(), req.DeclaredBalance);
            return Ok(new { success = true, message = "Caja cerrada y fondos trasladados a la sucursal." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public class OpenSessionRequest
{
    public int BranchId { get; set; }
    public decimal OpeningBalance { get; set; }
}

public class CloseSessionRequest
{
    public decimal DeclaredBalance { get; set; }
}
