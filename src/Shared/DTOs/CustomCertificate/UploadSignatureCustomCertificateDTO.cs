namespace api_camem.src.Shared.DTOs
{
    public class UploadSignatureCustomCertificateDTO : RequestDTO
    {
        public IFormFile Photo { get; set; }
    }
}
