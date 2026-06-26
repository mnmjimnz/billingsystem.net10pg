using BillingSystem.Application.Interfaces;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;
using BillingSystem.API.Hubs;

namespace BillingSystem.API.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notifRepo;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(INotificationRepository notifRepo, IHubContext<NotificationHub> hubContext)
    {
        _notifRepo = notifRepo;
        _hubContext = hubContext;
    }

    public async Task DispatchNotificationAsync(string title, string message, string type, int? referenceId)
    {
        // Notification is already saved in DB via SaleRepository.
        // We only dispatch to SignalR here.
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", new {
            title = title,
            message = message,
            type = type
        });
    }

    public async Task ResolveNotificationAsync(int referenceId, string type)
    {
        await _hubContext.Clients.All.SendAsync("NotificationResolved", referenceId);
    }
}
