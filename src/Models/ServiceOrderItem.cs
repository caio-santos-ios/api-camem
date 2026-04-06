using api_camem.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_camem.src.Models
{
    public class ServiceOrderItem : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("serviceOrderId")]
        public string ServiceOrderId { get; set; } = string.Empty;

        // service | part
        [BsonElement("itemType")]
        public string ItemType { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("productId")]
        public string ProductId { get; set; } = string.Empty;

        [BsonElement("isManual")]
        public bool IsManual { get; set; } = false;

        [BsonElement("quantity")]
        public decimal Quantity { get; set; } = 1;

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("cost")]
        public decimal Cost { get; set; }

        [BsonElement("total")]
        public decimal Total { get; set; }

        [BsonElement("supplierId")]
        public string SupplierId { get; set; } = string.Empty;

        [BsonElement("technicianId")]
        public string TechnicianId { get; set; } = string.Empty;

        [BsonElement("commission")]
        public decimal Commission { get; set; }

        [BsonElement("commissionType")]
        public string CommissionType { get; set; } = string.Empty; // percent | fixed
    }
}