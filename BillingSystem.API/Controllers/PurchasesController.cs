using BillingSystem.Application.DTOs;
using BillingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BillingSystem.API.Extensions;

namespace BillingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PurchasesController : ControllerBase
{
    private readonly IPurchaseService _service;

    public PurchasesController(IPurchaseService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(PurchaseDto dto)
    {
        try 
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "1");
            var id = await _service.CreatePurchaseAsync(dto, userId);
            return Ok(new { success = true, id });
        } 
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var purchaseData = await _service.GetPurchaseWithDetailsAsync(id); // Wait, PurchaseService doesn't have it
        if (purchaseData == null) return NotFound("Compra no encontrada");
        return Ok(purchaseData);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] string search = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] int? branchId = null)
    {
        if (!User.IsAdmin())
        {
            branchId = User.GetBranchId();
        }
        return Ok(await _service.GetPagedAsync(search, page, pageSize, branchId));
    }
}
