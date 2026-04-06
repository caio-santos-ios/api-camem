using System.ComponentModel.DataAnnotations;

namespace api_camem.src.Shared.DTOs
{
    public class CreateChartOfAccountsDTO
    {
        [Required(ErrorMessage = "Código é obrigatório")]
        [Display(Order = 1)]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nome é obrigatório")]
        [Display(Order = 2)]
        public string Name { get; set; } = string.Empty;

        [Display(Order = 3)]
        public string? ParentId { get; set; }

        [Required(ErrorMessage = "Tipo é obrigatório")]
        [Display(Order = 4)]
        public string Type { get; set; } = string.Empty;

        [Display(Order = 5)]
        public string? DreCategory { get; set; }

        [Display(Order = 6)]
        public bool ShowInDre { get; set; } = true;

        [Display(Order = 7)]
        public string Description { get; set; } = string.Empty;

        [Display(Order = 8)]
        public int Level { get; set; }

        [Display(Order = 9)]
        public bool IsAnalytical { get; set; } = false;
    }
}