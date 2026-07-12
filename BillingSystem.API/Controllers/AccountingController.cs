using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;

namespace BillingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountingController : ControllerBase
{
    private readonly IAccountingRepository _repo;

    public AccountingController(IAccountingRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccounts()
    {
        var accounts = await _repo.GetAccountsAsync();
        return Ok(accounts);
    }

    [HttpPost("accounts")]
    public async Task<IActionResult> AddAccount([FromBody] Account account)
    {
        var id = await _repo.AddAccountAsync(account);
        return Ok(new { Id = id });
    }

    [HttpPut("accounts/{id}")]
    public async Task<IActionResult> UpdateAccount(int id, [FromBody] Account account)
    {
        account.Id = id;
        await _repo.UpdateAccountAsync(account);
        return Ok();
    }

    [HttpGet("journal-entries")]
    public async Task<IActionResult> GetJournalEntries([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var entries = await _repo.GetJournalEntriesAsync(startDate, endDate);
        return Ok(entries);
    }

    [HttpGet("journal-entries/{id}")]
    public async Task<IActionResult> GetJournalEntry(int id)
    {
        var entry = await _repo.GetJournalEntryByIdAsync(id);
        if (entry == null) return NotFound();
        return Ok(entry);
    }

    [HttpGet("ledger/{accountId}")]
    public async Task<IActionResult> GetAccountLedger(int accountId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var ledger = await _repo.GetAccountLedgerAsync(accountId, startDate, endDate);
        return Ok(ledger);
    }

    [HttpGet("trial-balance")]
    public async Task<IActionResult> GetTrialBalance([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var start = startDate ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var end = endDate ?? DateTime.UtcNow;
        var balance = await _repo.GetTrialBalanceAsync(start, end);
        return Ok(balance);
    }
}
