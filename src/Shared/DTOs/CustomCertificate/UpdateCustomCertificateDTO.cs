using System.ComponentModel.DataAnnotations;

namespace api_camem.src.Shared.DTOs
{
    public class UpdateCustomCertificateDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "O HTML é obrigatório.")]
        [Display(Order = 1)]
        public string Html { get; set; } = string.Empty;
    }
}
