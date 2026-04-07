using api_camem.src.Enums.User;
using api_camem.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_camem.src.Models
{
    public class ProfileUser : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("code")]
        public string Code {get;set;} = string.Empty;
        
        [BsonElement("name")]
        public string Name {get;set;} = string.Empty;
        
        [BsonElement("description")]
        public string Description {get;set;} = string.Empty;
        
        [BsonElement("modules")]
        public List<Module> Modules {get;set;} = [];

        [BsonElement("role")]
        [BsonRepresentation(BsonType.String)] 
        public RoleEnum Role {get;set;} = RoleEnum.Student;
    }
}