namespace api_camem.src.Shared.DTOs
{
    public class UpdateUserStatusAccessDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        public string StatusAccess { get; set; } = string.Empty;
    }
}