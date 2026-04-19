namespace api_camem.src.Shared.DTOs
{
    public class CreateCertificateDTO : RequestDTO
    {
        public string Name { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public List<string> Functions { get; set; } = [];
        public decimal Hours { get; set; } = 0;
        public string KeyCertificate {get;set;} = string.Empty; 
        public string Html {get;set;} = string.Empty; 
    }
}