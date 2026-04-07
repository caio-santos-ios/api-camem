using api_camem.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using api_camem.src.Enums.User;

namespace api_camem.src.Models
{
    public class User : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("name")]
        public string Name {get;set;} = string.Empty;

        [BsonElement("userName")]
        public string UserName {get;set;} = string.Empty;
        
        [BsonElement("ra")]
        public string RA {get;set;} = string.Empty;
        
        [BsonElement("cpf")]
        public string Cpf {get;set;} = string.Empty;

        [BsonElement("email")]
        public string Email {get;set;} = string.Empty;

        [BsonElement("password")]
        public string Password {get;set;} = string.Empty;

        [BsonElement("role")]
        [BsonRepresentation(BsonType.String)] 
        public RoleEnum Role {get;set;} = RoleEnum.Student;

        [BsonElement("blocked")]
        public bool Blocked {get;set;} = false;
        
        [BsonElement("approved")]
        public bool Approved {get;set;} = false;
        
        [BsonElement("admin")]
        public bool Admin {get;set;} = false;
        
        [BsonElement("master")]
        public bool Master {get;set;} = false;
        
        [BsonElement("codeAccess")]
        public string CodeAccess {get;set;} = string.Empty;

        [BsonElement("validatedAccess")]
        public bool ValidatedAccess {get;set;} = false;

        [BsonElement("codeAccessExpiration")]
        public DateTime? CodeAccessExpiration { get; set; }

        [BsonElement("photo")]
        public string Photo {get;set;} = string.Empty;
        
        [BsonElement("modules")]
        public List<Module> Modules {get;set;} = [];
        
        [BsonElement("profileUserId")]
        public string ProfileUserId {get;set;} = string.Empty;
        
        [BsonElement("statusAccess")]
        public string StatusAccess {get;set;} = string.Empty;
    }

    public class Module 
    {
        [BsonElement("code")]
        public string Code {get;set;} = string.Empty;
        
        [BsonElement("description")]
        public string Description {get;set;} = string.Empty;
        
        [BsonElement("routines")]
        public List<Routine> Routines {get;set;} = [];
    }
    
    public class Routine 
    {
        [BsonElement("code")]
        public string Code {get;set;} = string.Empty;
        
        [BsonElement("description")]
        public string Description {get;set;} = string.Empty;

        [BsonElement("permissions")]
        public PermissionRoutine Permissions {get;set;} = new();
    }

    public class PermissionRoutine 
    {
        
        [BsonElement("read")]
        public bool Read {get;set;} = false;
        
        [BsonElement("create")]
        public bool Create {get;set;} = false;
        
        [BsonElement("update")]
        public bool Update {get;set;} = false;
        
        [BsonElement("delete")]
        public bool Delete {get;set;} = false;
    }
}