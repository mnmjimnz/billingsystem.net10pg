using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationRepository _notifRepo;

    public NotificationsController(INotificationRepository notifRepo)
    {
        _notifRepo = notifRepo;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var notifications = await _notifRepo.GetAllAsync();
        return Ok(notifications);
    }

    [HttpGet]
    public async Task<IActionResult> GetUnread()
    {
        var notifications = await _notifRepo.GetUnreadAsync();
        return Ok(notifications);
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _notifRepo.MarkAsReadAsync(id);
        return Ok();
    }
}
