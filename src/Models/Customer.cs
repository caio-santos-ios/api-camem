using api_camem.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_camem.src.Models
{
    public class Customer : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("corporateName")]
        public string CorporateName { get; set; } = string.Empty;
        
        [BsonElement("tradeName")]
        public string TradeName { get; set; } = string.Empty;
        
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("document")]
        public string Document { get; set; } = string.Empty; 

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;
    }
}