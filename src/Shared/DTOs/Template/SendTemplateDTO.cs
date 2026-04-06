using System.ComponentModel.DataAnnotations;

namespace api_camem.src.Shared.DTOs
{
    public class SendTemplateDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Código é obrigatório.")]
        [Display(Order = 1)]
        public string Code {get;set;} = string.Empty;
    }
}
