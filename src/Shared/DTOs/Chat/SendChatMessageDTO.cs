namespace api_camem.src.Shared.DTOs
{
    public class SendChatMessageDTO
    {
        public string SenderId   { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string SenderPhoto { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string Content    { get; set; } = string.Empty;
    }
}