using BillingSystem.Application.DTOs;
using BillingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "1");
        var id = await _service.CreatePurchaseAsync(dto, userId);
        return Ok(new { id });
    }
}
