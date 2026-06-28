using BillingSystem.Application.Interfaces;
using BillingSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockTransfersController : ControllerBase
{
    private readonly IStockTransferService _transferService;

    public StockTransfersController(IStockTransferService transferService)
    {
        _transferService = transferService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var transfers = await _transferService.GetAllTransfersAsync();
        return Ok(transfers);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StockTransfer transfer)
    {
        try
        {
            transfer.UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (transfer.UserId == 0) return Unauthorized();

            if (transfer.FromBranchId == transfer.ToBranchId)
                return BadRequest(new { message = "La sucursal de origen y destino no pueden ser la misma." });

            if (transfer.Quantity <= 0)
                return BadRequest(new { message = "La cantidad debe ser mayor a 0." });

            var id = await _transferService.TransferStockAsync(transfer);
            return Ok(new { message = "Traslado registrado exitosamente.", id = id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
