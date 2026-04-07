using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using api_camem.src.Enums.User;
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
        
        [Required(ErrorMessage = "O Role é obrigatório.")]
        [Display(Order = 2)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RoleEnum Role {get;set;} = RoleEnum.Student;
    }
}
