using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BranchesController : ControllerBase
{
    private readonly IBranchRepository _repo;

    public BranchesController(IBranchRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var branches = await _repo.GetAllAsync();
        return Ok(branches);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var branch = await _repo.GetByIdAsync(id);
        if (branch == null) return NotFound();
        return Ok(branch);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Branch branch)
    {
        branch.CreatedAt = DateTime.UtcNow;
        branch.IsActive = true;
        var id = await _repo.AddAsync(branch);
        return Ok(new { message = "Sucursal creada exitosamente", id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Branch branch)
    {
        branch.Id = id;
        branch.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(branch);
        return Ok(new { message = "Sucursal actualizada exitosamente" });
    }
}
