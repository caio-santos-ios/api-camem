using System.ComponentModel.DataAnnotations;

namespace api_camem.src.Shared.DTOs
{
    public class FinishEventDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Livro de Registro é obrigatório.")]
        [Display(Order = 1)]
        public string RegisterBookNumber {get;set;} = string.Empty;
    }
}
