using api_camem.src.Models;

namespace api_camem.src.Shared.DTOs
{
    public class UpdateUserSettingNotificationDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;

        public SettingNotification SettingNotification {get;set;} = new();
    }
}