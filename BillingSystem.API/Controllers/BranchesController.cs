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
    public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] string search = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(await _repo.GetPagedAsync(search, page, pageSize));
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

    [HttpPost("{id}/open")]
    public async Task<IActionResult> Open(int id)
    {
        var branch = await _repo.GetByIdAsync(id);
        if (branch == null) return NotFound(new { success = false, message = "Sucursal no encontrada" });
        if (branch.Status == "OPEN") return Ok(new { success = true, message = "La sucursal ya se encuentra abierta" });

        await _repo.UpdateStatusAsync(id, "OPEN");
        return Ok(new { success = true, message = "Sucursal aperturada exitosamente" });
    }

    [HttpPost("{id}/close")]
    public async Task<IActionResult> Close(int id)
    {
        var branch = await _repo.GetByIdAsync(id);
        if (branch == null) return NotFound(new { success = false, message = "Sucursal no encontrada" });
        if (branch.Status == "CLOSED") return Ok(new { success = true, message = "La sucursal ya se encuentra cerrada" });

        await _repo.UpdateStatusAsync(id, "CLOSED");
        return Ok(new { success = true, message = "Sucursal cerrada exitosamente" });
    }
}
