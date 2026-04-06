using api_camem.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_camem.src.Models
{
    public class Trigger : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("code")]
        public string Code { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("intervalValue")]
        public int IntervalValue { get; set; } = 1;

        /// <summary>minutes | hours | days</summary>
        [BsonElement("intervalUnit")]
        public string IntervalUnit { get; set; } = "hours";

        [BsonElement("lastFiredAt")]
        public DateTime? LastFiredAt { get; set; }

        [BsonElement("nextFireAt")]
        public DateTime? NextFireAt { get; set; }
    }
}