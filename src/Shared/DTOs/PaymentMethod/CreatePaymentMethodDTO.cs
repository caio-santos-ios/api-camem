using System.ComponentModel.DataAnnotations;
using api_camem.src.Models;

namespace api_camem.src.Shared.DTOs
{
    public class CreatePaymentMethodDTO : RequestDTO
    {
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [Display(Order = 1)]
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        [Required(ErrorMessage = "O Nº máximo de parcelas é obrigatório.")]
        [Display(Order = 2)]
        public int NumberOfInstallments { get; set; }
        public List<Interest> Interest {get;set;} = [];
    }
}