using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;

namespace BillingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BankController : ControllerBase
{
    private readonly IBankRepository _repo;

    public BankController(IBankRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("accounts")]
    public async Task<IActionResult> GetBankAccounts()
    {
        var accounts = await _repo.GetBankAccountsAsync();
        return Ok(accounts);
    }

    [HttpPost("accounts")]
    public async Task<IActionResult> AddBankAccount([FromBody] BankAccount account)
    {
        var id = await _repo.AddBankAccountAsync(account);
        return Ok(new { Id = id });
    }

    [HttpPut("accounts/{id}")]
    public async Task<IActionResult> UpdateBankAccount(int id, [FromBody] BankAccount account)
    {
        account.Id = id;
        await _repo.UpdateBankAccountAsync(account);
        return Ok();
    }

    [HttpGet("accounts/{id}/reconciliations")]
    public async Task<IActionResult> GetReconciliations(int id)
    {
        var recs = await _repo.GetReconciliationsAsync(id);
        return Ok(recs);
    }

    [HttpPost("reconciliations")]
    public async Task<IActionResult> AddReconciliation([FromBody] BankReconciliation rec, [FromQuery] IEnumerable<BankReconciliationDetail> details)
    {
        var id = await _repo.AddReconciliationAsync(rec, details);
        return Ok(new { Id = id });
    }
}
