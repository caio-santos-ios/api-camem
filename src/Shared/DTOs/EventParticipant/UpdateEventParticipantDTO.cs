using System.ComponentModel.DataAnnotations;
using api_camem.src.Models;

namespace api_camem.src.Shared.DTOs
{
    public class UpdateEventParticipantDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Nome no Certificado é obrigatório.")]
        [Display(Order = 1)]
        public string Name {get;set;} = string.Empty;        
        
        public string EventId {get;set;} = string.Empty;        
        
        public string UserId {get;set;} = string.Empty;        
        
        public List<EventParticipantFunction> Functions {get;set;} = []; 
    }
}
