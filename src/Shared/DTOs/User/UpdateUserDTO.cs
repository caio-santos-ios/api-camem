using System.ComponentModel.DataAnnotations;

namespace api_camem.src.Shared.DTOs
{
    public class UpdateUserDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [Display(Order = 1)]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "O E-mail é obrigatório.")]
        [Display(Order = 2)]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Perfil do Usuário é obrigatório.")]
        [Display(Order = 3)]
        public string ProfileUserId { get; set; } = string.Empty;
    
        public bool Admin { get; set; } = false;
        public bool Blocked { get; set; } = false;
        public List<ModuleCreateDTO> Modules {get;set;} = [];
        public DateTime? EffectiveDate { get; set; }
    }
}