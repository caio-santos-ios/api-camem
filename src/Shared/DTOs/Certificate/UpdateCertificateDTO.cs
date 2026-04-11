namespace api_camem.src.Shared.DTOs
{
    public class UpdateCertificateDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public List<string> Functions { get; set; } = [];
        public decimal Hours { get; set; } = 0;
    }
}
