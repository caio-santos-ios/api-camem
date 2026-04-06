using System.Security.Claims;
using api_camem.src.Interfaces;
using api_camem.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace api_camem.src.Hubs
{
    [Authorize]
    public class ChatHub(IChatService chatService, IHubContext<NotificationHub> notificationHub) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                // Grupo pessoal para receber mensagens diretas
                await Groups.AddToGroupAsync(Context.ConnectionId, $"chat_{userId}");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat_{userId}");
            }
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>Envia uma mensagem para outro usuário.</summary>
        public async Task SendMessage(SendChatMessageDTO dto)
        {
            string? senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? senderName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? Context.User?.FindFirst("name")?.Value ?? "Usuário";

            if (string.IsNullOrEmpty(senderId)) return;

            dto.SenderId   = senderId;
            dto.SenderName = senderName;

            var result = await chatService.SendMessageAsync(dto);
            if (!result.IsSuccess || result.Data is null) return;

            var message = result.Data;

            // Envia para o destinatário (se online)
            await Clients.Group($"chat_{dto.ReceiverId}").SendAsync("ReceiveMessage", message);

            // Confirma ao remetente
            await Clients.Caller.SendAsync("MessageSent", message);

            // Dispara notificação push para o destinatário via NotificationHub
            await notificationHub.Clients.Group(dto.ReceiverId).SendAsync("ReceiveNotification", new
            {
                title   = $"Nova mensagem de {senderName}",
                message = dto.Content.Length > 60 ? dto.Content[..60] + "..." : dto.Content,
                type    = "info",
                link    = "/chat"
            });
        }

        /// <summary>Marca mensagens de uma conversa como lidas.</summary>
        public async Task MarkConversationAsRead(string conversationId)
        {
            string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return;

            await chatService.MarkConversationAsReadAsync(conversationId, userId);
            await Clients.Caller.SendAsync("ConversationRead", conversationId);
        }

        /// <summary>Indica que o usuário está digitando.</summary>
        public async Task Typing(string receiverId)
        {
            string? senderId   = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? senderName = Context.User?.FindFirst("name")?.Value ?? "Usuário";
            if (string.IsNullOrEmpty(senderId)) return;

            await Clients.Group($"chat_{receiverId}").SendAsync("UserTyping", new { senderId, senderName });
        }

        /// <summary>Parou de digitar.</summary>
        public async Task StopTyping(string receiverId)
        {
            string? senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId)) return;

            await Clients.Group($"chat_{receiverId}").SendAsync("UserStopTyping", senderId);
        }
    }
}