using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierRepository _repo;
    public SuppliersController(ISupplierRepository repo) => _repo = repo;

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
        var supplier = await _repo.GetByIdAsync(id);
        return supplier == null ? NotFound() : Ok(supplier);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Supplier supplier) 
    {
        var id = await _repo.AddAsync(supplier);
        return Ok(new { id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Supplier supplier)
    {
        if (id != supplier.Id) return BadRequest();
        await _repo.UpdateAsync(supplier);
        return NoContent();
    }
}
