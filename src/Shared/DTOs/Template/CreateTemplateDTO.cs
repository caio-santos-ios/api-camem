using System.ComponentModel.DataAnnotations;

namespace api_camem.src.Shared.DTOs
{
    public class CreateTemplateDTO : RequestDTO
    {
        [Required(ErrorMessage = "O Código é obrigatório.")]
        [Display(Order = 1)]
        public string Code {get;set;} = string.Empty;
        
        [Required(ErrorMessage = "O HTML é obrigatório.")]
        [Display(Order = 2)]
        public string Html {get;set;} = string.Empty;
    }
}