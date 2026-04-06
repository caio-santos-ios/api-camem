using api_camem.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_camem.src.Models
{
    public class Count : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("collection")]
        public string Collection {get;set;} = string.Empty;        
        
        [BsonElement("index")]
        public int Index {get;set;} = 0;
    }
}