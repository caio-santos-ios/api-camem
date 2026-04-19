namespace api_camem.src.Shared.DTOs
{
    public class UpdatePresenceEventParticipantDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;     
        public string FunctionId {get;set;} = string.Empty;        
        public string NotesPresence {get;set;} = string.Empty;        
        public bool IsPresence {get;set;} = true;        
    }
}
