namespace api_camem.src.Shared.DTOs
{
    public class CreateNotificationDTO : RequestDTO
    {
        public string UserId  { get; set; } = string.Empty;
        public string Title   { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type    { get; set; } = "info";
        public string Link    { get; set; } = string.Empty;
    }
}