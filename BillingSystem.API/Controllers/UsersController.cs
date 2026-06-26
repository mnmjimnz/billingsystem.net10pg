using BillingSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repo;
    public UsersController(IUserRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BillingSystem.Domain.Entities.User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.IsActive = true;
        // Basic plain text for now to match AuthController
        var id = await _repo.AddAsync(user);
        return Ok(new { message = "Usuario creado exitosamente", id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] BillingSystem.Domain.Entities.User user)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return NotFound();

        existing.Username = user.Username;
        existing.FullName = user.FullName;
        existing.RoleId = user.RoleId;
        existing.BranchId = user.BranchId;
        if (!string.IsNullOrEmpty(user.PasswordHash))
        {
            existing.PasswordHash = user.PasswordHash;
        }
        existing.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(existing);
        return Ok(new { message = "Usuario actualizado exitosamente" });
    }
}
