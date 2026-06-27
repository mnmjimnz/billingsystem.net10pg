using BillingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class KardexController : ControllerBase
{
    private readonly IKardexService _kardexService;

    public KardexController(IKardexService kardexService)
    {
        _kardexService = kardexService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMovements([FromQuery] int? productId)
    {
        var result = await _kardexService.GetAllMovementsAsync(productId);
        return Ok(result);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] string search = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _kardexService.GetPagedAsync(search, page, pageSize);
        return Ok(result);
    }
}
