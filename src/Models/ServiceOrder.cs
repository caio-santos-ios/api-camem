using api_camem.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_camem.src.Models
{
    public class ServiceOrder : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("code")]
        public string Code { get; set; } = string.Empty;

        [BsonElement("customerId")]
        public string CustomerId { get; set; } = string.Empty;
        
        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("openingDate")]
        public DateTime OpeningDate { get; set; } = DateTime.UtcNow;
        
        [BsonElement("forecasDate")]
        public DateTime ForecasDate { get; set; } = DateTime.UtcNow;

        [BsonElement("status")]
        public string Status { get; set; } = "Em Aberto";

        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;

        [BsonElement("cancelReason")]
        public string CancelReason { get; set; } = string.Empty;

        [BsonElement("priority")]
        public string Priority { get; set; } = string.Empty;

        [BsonElement("Value")]
        public decimal Value { get; set; } = 0;

        [BsonElement("payment")]
        public ServiceOrderPayment Payment { get; set; } = new();
    }

    public class ServiceOrderPayment
    {
        [BsonElement("paymentMethodId")]
        public string PaymentMethodId { get; set; } = string.Empty;

        [BsonElement("numberOfInstallments")]
        public decimal NumberOfInstallments { get; set; } = 1;

        [BsonElement("paidAt")]
        public DateTime? PaidAt { get; set; }
    }
}