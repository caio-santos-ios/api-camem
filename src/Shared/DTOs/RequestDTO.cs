namespace api_camem.src.Shared.DTOs
{
    public class RequestDTO
    {
        public string CreatedBy {get;set;} = string.Empty;
        public string UpdatedBy {get;set;} = string.Empty;
        public string DeletedBy {get;set;} = string.Empty;
    }
}