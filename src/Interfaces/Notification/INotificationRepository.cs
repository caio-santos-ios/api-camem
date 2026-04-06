using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;

namespace api_camem.src.Interfaces
{
    public interface INotificationRepository
    {
        Task<ResponseApi<List<dynamic>>> GetByUserIdAsync(string userId, int limit = 30);
        Task<ResponseApi<int>> GetUnreadCountAsync(string userId);
        Task<ResponseApi<Notification?>> CreateAsync(Notification notification);
        Task<ResponseApi<Notification?>> MarkAsReadAsync(string notificationId, string userId);
        Task MarkAllAsReadAsync(string userId);
        Task<ResponseApi<Notification>> DeleteAsync(string notificationId, string userId);
    }
}