using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ThemesController : ControllerBase
{
    private readonly IThemeRepository _themeRepository;
    private readonly IThemeSettingRepository _themeSettingRepository;

    public ThemesController(IThemeRepository themeRepository, IThemeSettingRepository themeSettingRepository)
    {
        _themeRepository = themeRepository;
        _themeSettingRepository = themeSettingRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var themes = await _themeRepository.GetAllAsync();
        return Ok(themes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var theme = await _themeRepository.GetByIdAsync(id);
        if (theme == null) return NotFound();
        return Ok(theme);
    }

    [HttpPost("activate/{id}")]
    public async Task<IActionResult> Activate(int id)
    {
        await _themeRepository.ActivateThemeAsync(id);
        return Ok(new { message = "Theme activated successfully" });
    }

    [HttpGet("settings/{themeId}")]
    public async Task<IActionResult> GetSettings(int themeId)
    {
        var settings = await _themeSettingRepository.GetByThemeIdAsync(themeId);
        if (settings == null) return NotFound();
        return Ok(settings);
    }

    [HttpPut("settings/{themeId}")]
    public async Task<IActionResult> UpdateSettings(int themeId, [FromBody] ThemeSetting settings)
    {
        if (themeId != settings.ThemeId) return BadRequest("Theme ID mismatch");
        await _themeSettingRepository.UpdateAsync(settings);
        return Ok(new { message = "Settings updated successfully" });
    }
}
