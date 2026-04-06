using System.ComponentModel.DataAnnotations;
using api_camem.src.Models;

namespace api_camem.src.Shared.DTOs
{
    public class UpdateProfileUserDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [Display(Order = 1)]
        public string Name {get;set;} = string.Empty;
        public List<Module> Modules {get;set;} = [];
    }
}
