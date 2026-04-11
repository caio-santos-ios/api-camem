using System.ComponentModel.DataAnnotations;

namespace api_camem.src.Shared.DTOs
{
    public class UpdateEventDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Título é obrigatório.")]
        [Display(Order = 1)]
        public string Title {get;set;} = string.Empty;
        
        [Required(ErrorMessage = "A Data Inicial é obrigatória.")]
        [Display(Order = 2)]
        public DateTime StartDate {get;set;}
        public DateTime? EndDate {get;set;}        
        
        [Required(ErrorMessage = "A Descrição é obrigatória.")]
        [Display(Order = 3)]
        public string Description {get;set;} = string.Empty;
    }
}
