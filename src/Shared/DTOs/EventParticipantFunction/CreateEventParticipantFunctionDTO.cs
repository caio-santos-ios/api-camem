using System.ComponentModel.DataAnnotations;

namespace api_camem.src.Shared.DTOs
{
    public class CreateEventParticipantFunctionDTO : RequestDTO
    {
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [Display(Order = 1)]
        public string Name {get;set;} = string.Empty;        
        
        public string EventId {get;set;} = string.Empty;
        
        public string EventParticipantId {get;set;} = string.Empty;  

        [Required(ErrorMessage = "As Horas são obrigatórias.")]
        [Display(Order = 2)]
        public decimal Hours {get;set;} = 0;       
        
        public bool IsPresence {get;set;} = true;   

        public string NotesPresence {get;set;} = string.Empty; 
    }
}