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
    private readonly BillingSystem.Domain.Interfaces.ICashRegisterRepository _cashRepo;

    public CashRegistersController(ICashRegisterService cashService, BillingSystem.Domain.Interfaces.ICashRegisterRepository cashRepo)
    {
        _cashService = cashService;
        _cashRepo = cashRepo;
    }

    private int GetCurrentUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpGet("session")]
    public async Task<IActionResult> GetActiveSession()
    {
        var session = await _cashService.GetActiveSessionAsync(GetCurrentUserId());
        int? branchId = null;
        if (session != null)
        {
            var register = await _cashRepo.GetByIdAsync(session.CashRegisterId);
            branchId = register?.BranchId;
        }
        return Ok(new { success = true, hasOpenSession = session != null, session, branchId });
    }

    [HttpGet("branch/{branchId}")]
    public async Task<IActionResult> GetByBranch(int branchId)
    {
        try
        {
            var registers = await _cashService.GetRegistersByBranchAsync(branchId);
            return Ok(new { success = true, data = registers });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("open")]
    public async Task<IActionResult> OpenSession([FromBody] OpenSessionRequest req)
    {
        try
        {
            var id = await _cashService.OpenSessionAsync(GetCurrentUserId(), req.CashRegisterId, req.OpeningBalance);
            return Ok(new { success = true, sessionId = id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("close")]
    public async Task<IActionResult> CloseSession()
    {
        try
        {
            await _cashService.CloseSessionAsync(GetCurrentUserId());
            return Ok(new { success = true, message = "Caja cerrada y fondos trasladados a la sucursal." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("session-summary")]
    public async Task<IActionResult> GetSessionSummary()
    {
        try
        {
            var summary = await _cashService.GetSessionSummaryAsync(GetCurrentUserId());
            if (summary == null) return Ok(new { success = false, message = "No hay caja abierta." });
            return Ok(new { success = true, data = summary });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public class OpenSessionRequest
{
    public int CashRegisterId { get; set; }
    public decimal OpeningBalance { get; set; }
}

public class CloseSessionRequest
{
    public decimal DeclaredBalance { get; set; }
}
