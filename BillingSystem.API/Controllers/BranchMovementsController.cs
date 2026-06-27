using BillingSystem.Application.Interfaces;
using BillingSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BranchMovementsController : ControllerBase
{
    private readonly IBranchMovementService _movementService;

    public BranchMovementsController(IBranchMovementService movementService)
    {
        _movementService = movementService;
    }

    [HttpGet("branch/{branchId}")]
    public async Task<IActionResult> GetByBranch(int branchId)
    {
        var movements = await _movementService.GetMovementsByBranchIdAsync(branchId);
        return Ok(movements);
    }

    [HttpPost]
    public async Task<IActionResult> RegisterMovement([FromBody] BranchMovement movement)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int userId))
        {
            movement.UserId = userId;
        }
        else
        {
            return Unauthorized("Token de usuario inválido.");
        }

        var result = await _movementService.RegisterMovementAsync(movement);
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return BadRequest(new { message = result.Error });
    }
}
