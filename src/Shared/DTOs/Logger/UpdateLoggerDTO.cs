namespace api_camem.src.Shared.DTOs
{
    public class UpdateLoggerDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Method {get;set;} = string.Empty;
        public int StatusCode {get;set;} = 0;
        public string Message {get;set;} = string.Empty;
    }
}
