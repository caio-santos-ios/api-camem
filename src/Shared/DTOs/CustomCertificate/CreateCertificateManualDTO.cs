namespace api_camem.src.Shared.DTOs.CustomCertificate
{
    public class CreateCertificateManualDTO : RequestDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string NameEvent { get; set; } = string.Empty;
        public string RegisterBookNumberEvent { get; set; } = string.Empty;
        public string RegisterFolderNumberEvent { get; set; } = string.Empty;
        public string KeyCertificateEvent { get; set; } = string.Empty;
        public string NameParticipantEvent { get; set; } = string.Empty;
        public DateTime StartDateEvent { get; set; }
        public DateTime EndDateEvent { get; set; }
        public string ParticipantFunction { get; set; } = string.Empty;
        public IFormFile? Certificate { get; set; }
    }

    public class CreateCertificateManualParticipantFunction
    {
        public string Name { get; set; } = string.Empty;
        public int Hours { get; set; }
    }
}