using System.ComponentModel.DataAnnotations;

namespace api_camem.src.Shared.DTOs
{
    public class UpdateStatusServiceOrderDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "O Status é obrigatório.")]
        [Display(Order = 1)]
        public string Status { get; set; } = string.Empty;
    }
}
