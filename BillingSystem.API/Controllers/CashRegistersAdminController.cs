using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BillingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CashRegistersAdminController : ControllerBase
{
    private readonly ICashRegisterRepository _repo;

    public CashRegistersAdminController(ICashRegisterRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("branch/{branchId}")]
    public async Task<IActionResult> GetByBranch(int branchId)
    {
        var registers = await _repo.GetByBranchIdAsync(branchId);
        return Ok(registers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var register = await _repo.GetByIdAsync(id);
        if (register == null) return NotFound();
        return Ok(register);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CashRegister register)
    {
        register.CreatedAt = DateTime.UtcNow;
        register.IsActive = true;
        var id = await _repo.AddAsync(register);
        return Ok(new { message = "Caja creada exitosamente", id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CashRegister register)
    {
        register.Id = id;
        register.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(register);
        return Ok(new { message = "Caja actualizada exitosamente" });
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] JsonElement payload)
    {
        bool isActive = payload.GetProperty("isActive").GetBoolean();
        await _repo.UpdateStatusAsync(id, isActive);
        return Ok(new { message = "Estado actualizado exitosamente" });
    }
}
