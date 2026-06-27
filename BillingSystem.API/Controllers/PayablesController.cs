using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PayablesController : ControllerBase
{
    private readonly IPayableRepository _repo;

    public PayablesController(IPayableRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var pending = await _repo.GetPendingAsync();
        return Ok(pending);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] string search = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(await _repo.GetPagedAsync(search, page, pageSize));
    }

    [HttpPost("{id}/payments")]
    public async Task<IActionResult> AddPayment(int id, [FromBody] PayablePayment payment)
    {
        var account = await _repo.GetAccountByIdAsync(id);
        if (account == null) return NotFound("Cuenta por pagar no encontrada");

        if (payment.Amount <= 0 || payment.Amount > account.Balance)
            return BadRequest("Monto inválido");

        payment.AccountId = id;
        payment.UserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

        await _repo.AddPaymentAsync(payment);
        await _repo.UpdateAccountBalanceAsync(id, payment.Amount);

        return Ok(new { message = "Pago registrado exitosamente" });
    }
}
