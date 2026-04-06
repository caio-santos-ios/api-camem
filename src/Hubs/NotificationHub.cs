using System.Security.Claims;
using api_camem.src.Interfaces;
using api_camem.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace api_camem.src.Hubs
{
    [Authorize]
    public class NotificationHub(INotificationService notificationService) : Hub
    {
        /// <summary>
        /// Ao conectar, adiciona o usuário ao seu grupo pessoal (userId).
        /// Isso permite enviar notificações diretas: Clients.Group(userId).
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>Marca uma notificação como lida pelo cliente.</summary>
        public async Task MarkAsRead(string notificationId)
        {
            string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return;

            await notificationService.MarkAsReadAsync(notificationId, userId);

            // Confirma ao cliente que foi lida
            await Clients.Caller.SendAsync("NotificationRead", notificationId);
        }

        /// <summary>Marca todas as notificações do usuário como lidas.</summary>
        public async Task MarkAllAsRead()
        {
            string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return;

            await notificationService.MarkAllAsReadAsync(userId);
            await Clients.Caller.SendAsync("AllNotificationsRead");
        }
    }
}