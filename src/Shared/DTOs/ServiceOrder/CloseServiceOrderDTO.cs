namespace api_camem.src.Shared.DTOs
{
    public class CloseServiceOrderDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        public string ClosedByUserId { get; set; } = string.Empty;
        public int WarrantyDays { get; set; } = 90;
        public DateTime? WarrantyUntil { get; set; }

        public string PaymentMethodId { get; set; } = string.Empty;
        public string PaymentMethodName { get; set; } = string.Empty;
        public int NumberOfInstallments { get; set; } = 1;
        public decimal DiscountValue { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }
}
