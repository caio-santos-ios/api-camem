using System.ComponentModel.DataAnnotations;

namespace api_camem.src.Shared.DTOs
{
    public class CreateServiceOrderDTO : RequestDTO
    {
        [Required(ErrorMessage = "O Cliente é obrigatório.")]
        [Display(Order = 1)]
        public string CustomerId { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Data de Abertura é obrigatória.")]
        [Display(Order = 2)]
        public string OpeningDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Previsão de Entrega é obrigatória.")]
        [Display(Order = 3)]
        public string ForecasDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Prioridade é obrigatória.")]
        [Display(Order = 4)]
        public string Priority { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Descrição é obrigatória.")]
        [Display(Order = 5)]
        public string Description { get; set; } = string.Empty;
    }
}
