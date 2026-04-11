using api_camem.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_camem.src.Models
{
    public class Event : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("title")]
        public string Title {get;set;} = string.Empty;        
        
        [BsonElement("description")]
        public string Description {get;set;} = string.Empty;        
        
        [BsonElement("status")]
        public string Status {get;set;} = string.Empty;        
        
        [BsonElement("startDate")]
        public DateTime StartDate {get;set;}

        [BsonElement("endDate")]
        public DateTime? EndDate {get;set;}   
        
        [BsonElement("userIds")]
        public List<string> UserIds {get;set;} = [];
    }
}