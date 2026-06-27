using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly ISettingsRepository _repo;

    public SettingsController(ISettingsRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    [AllowAnonymous] // Permite obtener nombre de la empresa sin login para el login visual (opcional)
    public async Task<IActionResult> GetSettings()
    {
        return Ok(await _repo.GetSettingsAsync());
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSettings([FromBody] CompanySetting settings)
    {
        await _repo.UpdateSettingsAsync(settings);
        return Ok(new { success = true, message = "Configuración actualizada correctamente" });
    }
}
