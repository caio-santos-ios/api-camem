using api_camem.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_camem.src.Models
{
    public class CustomCertificate : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("html")]
        public string Html { get; set; } = string.Empty;

        [BsonElement("bodyText")]
        public string BodyText { get; set; } = string.Empty;

        [BsonElement("signer1Name")]
        public string Signer1Name { get; set; } = string.Empty;

        [BsonElement("signer1Role")]
        public string Signer1Role { get; set; } = string.Empty;

        [BsonElement("signer1Photo")]
        public string Signer1Photo { get; set; } = string.Empty;

        [BsonElement("signer2Name")]
        public string Signer2Name { get; set; } = string.Empty;

        [BsonElement("signer2Role")]
        public string Signer2Role { get; set; } = string.Empty;

        [BsonElement("signer2Photo")]
        public string Signer2Photo { get; set; } = string.Empty;

        [BsonElement("signer3Name")]
        public string Signer3Name { get; set; } = string.Empty;

        [BsonElement("signer3Role")]
        public string Signer3Role { get; set; } = string.Empty;

        [BsonElement("signer3Photo")]
        public string Signer3Photo { get; set; } = string.Empty;
    }
}