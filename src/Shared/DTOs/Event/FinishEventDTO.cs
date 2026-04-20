using System.ComponentModel.DataAnnotations;

namespace api_camem.src.Shared.DTOs
{
    public class FinishEventDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Livro de Registro é obrigatório.")]
        [Display(Order = 1)]
        public string RegisterBookNumber { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "A Pasta de Registro é obrigatória.")]
        [Display(Order = 2)]
        public string RegisterFolderNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Certificado é obrigatório.")]
        [Display(Order = 3)]
        public string CertificateId { get; set; } = string.Empty;
    }
}
