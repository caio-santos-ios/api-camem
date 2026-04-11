using api_camem.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_camem.src.Models
{
    public class EventParticipant : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("name")]
        public string Name {get;set;} = string.Empty;        
        
        [BsonElement("eventId")]
        public string EventId {get;set;} = string.Empty;        
        
        [BsonElement("userId")]
        public string UserId {get;set;} = string.Empty;        
        
        [BsonElement("functions")]
        public List<EventParticipantFunction> Functions {get;set;} = [];        
    }
    public class EventParticipantFunction : ModelBase
    {
        [BsonElement("name")]
        public string Name {get;set;} = string.Empty;        
        
        [BsonElement("hours")]
        public decimal Hours {get;set;} = 0;       
        
        [BsonElement("isPresence")]
        public bool IsPresence {get;set;} = true;   

        [BsonElement("notesPresence")]
        public string NotesPresence {get;set;} = string.Empty;       
    }
}