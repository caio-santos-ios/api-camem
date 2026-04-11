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

        [Required(ErrorMessage = "O Vínculo institucional é obrigatório.")]
        [Display(Order = 3)]
        public string ProfileUserId { get; set; } = string.Empty;    
        
        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [Display(Order = 4)]
        public string Cpf { get; set; } = string.Empty;    
        
        [Required(ErrorMessage = "O RA é obrigatório.")]
        [Display(Order = 5)]
        public string RA { get; set; } = string.Empty;    
        public bool Blocked { get; set; } = false;
    }
}