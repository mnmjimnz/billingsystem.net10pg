using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Transactions;

namespace BillingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PayablesController : ControllerBase
{
    private readonly IPayableRepository _repo;
    private readonly IBranchRepository _branchRepo;

    public PayablesController(IPayableRepository repo, IBranchRepository branchRepo)
    {
        _repo = repo;
        _branchRepo = branchRepo;
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

        var branchIdStr = User.FindFirst("BranchId")?.Value;
        if (string.IsNullOrEmpty(branchIdStr) || !int.TryParse(branchIdStr, out int userBranchId))
            return BadRequest("Usuario no tiene sucursal asignada.");

        var branch = await _branchRepo.GetByIdAsync(userBranchId);
        if (branch == null) return BadRequest("Sucursal no encontrada.");

        if (branch.AvailableFunds < payment.Amount)
            return BadRequest(new { message = $"La sucursal no tiene fondos suficientes. Saldo disponible: ${branch.AvailableFunds:F2}" });

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        payment.AccountId = id;
        payment.UserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

        await _repo.AddPaymentAsync(payment);
        await _repo.UpdateAccountBalanceAsync(id, payment.Amount);

        branch.AvailableFunds -= payment.Amount;
        await _branchRepo.UpdateAsync(branch);

        scope.Complete();

        return Ok(new { message = "Pago registrado exitosamente" });
    }

    [HttpGet("{id}/payments")]
    public async Task<IActionResult> GetPayments(int id)
    {
        var account = await _repo.GetAccountByIdAsync(id);
        if (account == null) return NotFound("Cuenta por pagar no encontrada");

        var payments = await _repo.GetPaymentsAsync(id);
        return Ok(payments);
    }
}
