using api_camem.src.Hubs;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace api_camem.src.Services
{
    public class NotificationService(
        INotificationRepository repository,
        IHubContext<NotificationHub> hubContext
    ) : INotificationService
    {
        public async Task<ResponseApi<List<dynamic>>> GetByUserIdAsync(string userId, int limit = 30)
            => await repository.GetByUserIdAsync(userId, limit);

        public async Task<ResponseApi<int>> GetUnreadCountAsync(string userId)
            => await repository.GetUnreadCountAsync(userId);

        public async Task<ResponseApi<Notification?>> SendToUserAsync(CreateNotificationDTO dto)
        {
            try
            {
                var notification = new Notification
                {
                    UserId    = dto.UserId,
                    Title     = dto.Title,
                    Message   = dto.Message,
                    Type      = dto.Type,
                    Link      = dto.Link,
                    CreatedBy = dto.CreatedBy,
                };

                var response = await repository.CreateAsync(notification);
                if (!response.IsSuccess || response.Data is null)
                    return new(null, 500, "Falha ao criar notificação.");

                // Envia em tempo real via SignalR para o grupo do usuário
                await hubContext.Clients.Group(dto.UserId).SendAsync("ReceiveNotification", new
                {
                    id      = response.Data.Id,
                    title   = dto.Title,
                    message = dto.Message,
                    type    = dto.Type,
                    link    = dto.Link,
                    read    = false,
                    createdAt = response.Data.CreatedAt,
                });

                return response;
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado.");
            }
        }

        public async Task SendToManyAsync(IEnumerable<string> userIds, CreateNotificationDTO dto)
        {
            foreach (var userId in userIds)
            {
                await SendToUserAsync(new()
                {
                    UserId    = userId,
                    CreatedBy = userId,
                    Title     = dto.Title,
                    Message   = dto.Message,
                    Type      = dto.Type,
                    Link      = dto.Link,
                });
            }
        }
        public async Task<ResponseApi<Notification?>> MarkAsReadAsync(string notificationId, string userId)
            => await repository.MarkAsReadAsync(notificationId, userId);

        public async Task MarkAllAsReadAsync(string userId)
            => await repository.MarkAllAsReadAsync(userId);

        public async Task<ResponseApi<Notification>> DeleteAsync(string notificationId, string userId)
            => await repository.DeleteAsync(notificationId, userId);
    }
}