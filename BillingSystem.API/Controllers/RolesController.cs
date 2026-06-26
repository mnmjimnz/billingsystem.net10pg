using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRoleRepository _repo;

    public RolesController(IRoleRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _repo.GetAllAsync();
        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var role = await _repo.GetByIdAsync(id);
        if (role == null) return NotFound();
        return Ok(role);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Role role)
    {
        role.CreatedAt = DateTime.UtcNow;
        role.IsActive = true;
        var id = await _repo.AddAsync(role);
        return Ok(new { message = "Rol creado exitosamente", id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Role role)
    {
        role.Id = id;
        role.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(role);
        return Ok(new { message = "Rol actualizado exitosamente" });
    }

    [HttpGet("permissions")]
    public async Task<IActionResult> GetAllPermissions()
    {
        return Ok(await _repo.GetAllPermissionsAsync());
    }

    [HttpGet("{id}/permissions")]
    public async Task<IActionResult> GetRolePermissions(int id)
    {
        return Ok(await _repo.GetPermissionsByRoleIdAsync(id));
    }

    [HttpPost("{id}/permissions")]
    public async Task<IActionResult> AssignPermissions(int id, [FromBody] List<int> permissionIds)
    {
        await _repo.AssignPermissionsToRoleAsync(id, permissionIds);
        return Ok(new { message = "Permisos asignados exitosamente" });
    }
}
