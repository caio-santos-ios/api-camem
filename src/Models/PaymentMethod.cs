using api_camem.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_camem.src.Models
{
    public class PaymentMethod : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("code")]
        public string Code { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("numberOfInstallments")]
        public int NumberOfInstallments { get; set; }

        [BsonElement("interest")]
        public List<Interest> Interest {get;set;} = [];
    }

    public class Interest 
    {
        [BsonElement("installment")]
        public int Installment { get; set; }

        [BsonElement("transactionFee")]
        public decimal TransactionFee { get; set; }

        [BsonElement("surcharge")]
        public decimal Surcharge { get; set; }
    }
}