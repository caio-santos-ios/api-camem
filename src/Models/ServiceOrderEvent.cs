using api_camem.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_camem.src.Models
{
    public class ServiceOrderEvent : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("serviceOrderId")]
        public string ServiceOrderId { get; set; } = string.Empty;

        // status_change | item_added | item_removed | laudo_updated | note | payment | warranty_override
        [BsonElement("eventType")]
        public string EventType { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("oldValue")]
        public string OldValue { get; set; } = string.Empty;

        [BsonElement("newValue")]
        public string NewValue { get; set; } = string.Empty;
    }
}
