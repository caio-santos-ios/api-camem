namespace api_camem.src.Shared.DTOs
{
    public class CreateLoggerDTO : RequestDTO
    {
        public string Path {get;set;} = string.Empty;
        public string Method {get;set;} = string.Empty;
        public int StatusCode {get;set;} = 0;
        public string Message {get;set;} = string.Empty;
        public double Time {get;set;} = 0;
        public bool Audit {get;set;} = false;
    }
}