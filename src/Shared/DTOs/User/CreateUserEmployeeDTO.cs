using System.ComponentModel.DataAnnotations;

namespace api_camem.src.Shared.DTOs
{
    public class CreateUserEmployeeDTO : RequestDTO
    {
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [Display(Order = 1)]
        public string Name {get;set;} = string.Empty;
        
        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [Display(Order = 2)]
        public string CPF {get;set;} = string.Empty;

        [Required(ErrorMessage = "O E-mail é obrigatório.")]
        [Display(Order = 3)]
        public string Email {get;set;} = string.Empty;

        [Required(ErrorMessage = "O Telefone é obrigatório.")]
        [Display(Order = 4)]
        public string Phone {get;set;} = string.Empty;
        public string Rg {get;set;} = string.Empty;
        public string Whatsapp {get;set;} = string.Empty;
        public bool Blocked {get;set;} = false;
        public bool Admin {get;set;} = false;
        public string CodeAccess {get;set;} = string.Empty;
        public bool ValidatedAccess {get;set;} = false;
        public DateTime? CodeAccessExpiration { get; set; }
        public string Photo {get;set;} = string.Empty;
        public List<string> Companies {get;set;} = [];
        public List<string> Stores {get;set;} = [];
        public List<ModuleCreateDTO> Modules {get;set;} = [];
        
        [Required(ErrorMessage = "O Tipo é obrigatório.")]
        [Display(Order = 5)]
        public string Type { get; set; } = string.Empty; 
        public DateTime? DateOfBirth { get; set; } 
    }
}