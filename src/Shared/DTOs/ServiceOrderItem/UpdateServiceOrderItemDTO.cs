namespace api_camem.src.Shared.DTOs
{
    public class UpdateServiceOrderItemDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        public string ServiceOrderId { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public bool IsManual { get; set; } = false;
        public decimal Quantity { get; set; } = 1;
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public decimal Total { get; set; }
        public string SupplierId { get; set; } = string.Empty;
        public string TechnicianId { get; set; } = string.Empty;
        public decimal Commission { get; set; }
        public string CommissionType { get; set; } = string.Empty;
    }
}
