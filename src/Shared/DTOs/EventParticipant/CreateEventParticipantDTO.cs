using System.ComponentModel.DataAnnotations;

namespace api_camem.src.Shared.DTOs
{
    public class CreateEventParticipantDTO : RequestDTO
    {
        [Required(ErrorMessage = "O Nome no Certificado é obrigatório.")]
        [Display(Order = 1)]
        public string Name {get;set;} = string.Empty;        
        
        public string EventId {get;set;} = string.Empty;        
        
        public string UserId {get;set;} = string.Empty;

        [Required(ErrorMessage = "A Função é obrigatória.")]
        [Display(Order = 1)]
        public string FunctionId {get;set;} = string.Empty;
        public decimal Hours {get;set;} = 0;           
    }
}