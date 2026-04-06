namespace api_camem.src.Shared.DTOs
{
    public class UpdateCustomerMinimalDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string CorporateName { get; set; } = string.Empty;
        public string TradeName { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}