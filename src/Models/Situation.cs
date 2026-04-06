using api_camem.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_camem.src.Models
{
    public class Situation : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("code")]
        public string Code {get; set;} = string.Empty;
        
        [BsonElement("name")]
        public string Name {get; set;} = string.Empty;

        [BsonElement("style")]
        public Style Style {get; set;} = new();
        
        [BsonElement("start")]
        public bool Start {get; set;}
        
        [BsonElement("end")]
        public bool End {get; set;}
        
        [BsonElement("quite")]
        public bool Quite {get; set;}
        
        [BsonElement("generateFinancial")]
        public bool GenerateFinancial {get; set;}
        
        [BsonElement("appearsOnPanel")]
        public bool AppearsOnPanel {get; set;}

        [BsonElement("sequence")]
        public int Sequence {get; set;}
    }

    public class Style
    {
        [BsonElement("bg")]
        public string Bg {get; set;} = string.Empty;
        
        [BsonElement("border")]
        public string Border {get; set;} = string.Empty;
        
        [BsonElement("text")]
        public string Text {get; set;} = string.Empty;
    }
}