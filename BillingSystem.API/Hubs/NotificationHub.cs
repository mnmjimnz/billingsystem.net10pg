using Microsoft.AspNetCore.SignalR;

namespace BillingSystem.API.Hubs;

public class NotificationHub : Hub
{
    // Cliente puede enviar mensajes al hub si es necesario, 
    // pero usualmente el servidor despacha las notificaciones.
    public async Task SendNotification(string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", message);
    }
}
