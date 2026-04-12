using api_camem.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_camem.src.Models
{
    public class Certificate : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;
        
        [BsonElement("eventId")]
        public string EventId { get; set; } = string.Empty;

        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("functions")]
        public List<string> Functions { get; set; } = [];

        [BsonElement("hours")]
        public decimal Hours { get; set; } = 0;
        
        [BsonElement("keyCertificate")]
        public string KeyCertificate {get;set;} = string.Empty;    
    }
}