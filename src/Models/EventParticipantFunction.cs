using api_camem.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_camem.src.Models
{
    public class EventParticipantFunction : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("name")]
        public string Name {get;set;} = string.Empty;        
        
        [BsonElement("eventId")]
        public string EventId {get;set;} = string.Empty;   
        
        [BsonElement("eventParticipantId")]
        public string EventParticipantId {get;set;} = string.Empty; 
        
        [BsonElement("hours")]
        public decimal Hours {get;set;} = 0;       
        
        [BsonElement("isPresence")]
        public bool IsPresence {get;set;} = true;   

        [BsonElement("notesPresence")]
        public string NotesPresence {get;set;} = string.Empty;    
    }
}