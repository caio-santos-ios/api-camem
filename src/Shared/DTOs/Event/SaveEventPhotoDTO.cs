namespace api_camem.src.Shared.DTOs
{
    public class SaveEventPhotoDTO
    {
        public string Id { get; set; } = string.Empty;
        public IFormFile? Photo { get; set; }
    }
}