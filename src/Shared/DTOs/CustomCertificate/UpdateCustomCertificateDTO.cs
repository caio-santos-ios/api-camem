using System.ComponentModel.DataAnnotations;

namespace api_camem.src.Shared.DTOs
{
    public class UpdateCustomCertificateDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [Display(Order = 1)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "O HTML é obrigatório.")]
        [Display(Order = 2)]
        public string Html { get; set; } = string.Empty;

        public string BodyText { get; set; } = string.Empty;
        public string Signer1Name { get; set; } = string.Empty;
        public string Signer1Role { get; set; } = string.Empty;
        public string Signer1Photo { get; set; } = string.Empty;
        public string Signer2Name { get; set; } = string.Empty;
        public string Signer2Role { get; set; } = string.Empty;
        public string Signer2Photo { get; set; } = string.Empty;
        public string Signer3Name { get; set; } = string.Empty;
        public string Signer3Role { get; set; } = string.Empty;
        public string Signer3Photo { get; set; } = string.Empty;
    }
}
