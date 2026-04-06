using api_camem.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_camem.src.Models
{
    public class Notification : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("message")]
        public string Message { get; set; } = string.Empty;

        [BsonElement("type")]
        public string Type { get; set; } = "info";

        [BsonElement("read")]
        public bool Read { get; set; } = false;

        [BsonElement("readAt")]
        public DateTime? ReadAt { get; set; }

        [BsonElement("link")]
        public string Link { get; set; } = string.Empty;
    }
}