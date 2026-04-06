using System.ComponentModel.DataAnnotations;

namespace api_camem.src.Shared.DTOs
{
    public class CreateUserDTO : RequestDTO
    {
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [Display(Order = 1)]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "O E-mail é obrigatório.")]
        [Display(Order = 2)]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "A Senha é obrigatória.")]
        [Display(Order = 3)]    
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Perfil do Usuário é obrigatório.")]
        [Display(Order = 4)]    
        public string ProfileUserId { get; set; } = string.Empty;
        public bool Admin { get; set; } = false;
        public bool Blocked { get; set; } = false;
        public List<ModuleCreateDTO> Modules {get;set;} = [];
        public DateTime? EffectiveDate { get; set; }
    }

    public class ModuleCreateDTO 
    {
        public string Code {get;set;} = string.Empty;
        public string Description {get;set;} = string.Empty;        
        public List<Routine> Routines {get;set;} = [];
    }
    
    public class Routine 
    {
        public string Code {get;set;} = string.Empty;
        
        public string Description {get;set;} = string.Empty;

        public PermissionRoutine Permissions {get;set;} = new();
    }

    public class PermissionRoutine 
    {
        public bool Read {get;set;} = false;   
        public bool Create {get;set;} = false;
        public bool Update {get;set;} = false;
        public bool Delete {get;set;} = false;
    }
}