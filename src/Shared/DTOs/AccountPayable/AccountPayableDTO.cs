using System.ComponentModel.DataAnnotations;

namespace api_camem.src.Shared.DTOs
{
    public class CreateAccountPayableDTO : RequestDTO
    {
        [Required(ErrorMessage = "A Descrição é obrigatória.")]
        [Display(Order = 1)]
        public string Description { get; set; } = string.Empty;

        public string OriginId { get; set; } = string.Empty;

        public string OriginType { get; set; } = "manual";

        public string SupplierId { get; set; } = string.Empty;

        public string PaymentMethodId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Valor é obrigatório.")]
        [Display(Order = 2)]
        public decimal Amount { get; set; }

        public int InstallmentNumber { get; set; } = 1;

        public int TotalInstallments { get; set; } = 1;

        [Required(ErrorMessage = "A Data de Vencimento é obrigatória.")]
        [Display(Order = 3)]
        public DateTime DueDate { get; set; }
        
        [Required(ErrorMessage = "A Data de Emissão é obrigatória.")]
        [Display(Order = 3)]
        public DateTime IssueDate { get; set; }

        public string Notes { get; set; } = string.Empty;
        public string ChartOfAccountId { get; set; } = string.Empty;
        public bool IsRecurrent { get; set; } = false;
        public string TypeRecurrent { get; set; } = string.Empty;
    }

    public class UpdateAccountPayableDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Descrição é obrigatória.")]
        [Display(Order = 1)]
        public string Description { get; set; } = string.Empty;

        public string OriginId { get; set; } = string.Empty;

        public string OriginType { get; set; } = "manual";

        public string SupplierId { get; set; } = string.Empty;
        public string PaymentMethodId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Valor é obrigatório.")]
        [Display(Order = 2)]
        public decimal Amount { get; set; }

        public int InstallmentNumber { get; set; } = 1;

        public int TotalInstallments { get; set; } = 1;

        [Required(ErrorMessage = "A Data de Vencimento é obrigatória.")]
        [Display(Order = 3)]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "A Data de Emissão é obrigatória.")]
        [Display(Order = 3)]
        public DateTime IssueDate { get; set; }

        public string Notes { get; set; } = string.Empty;
        public string ChartOfAccountId { get; set; } = string.Empty;
        public bool IsRecurrent { get; set; } = false;
        public string TypeRecurrent { get; set; } = string.Empty;
    }

    public class PayAccountPayableDTO
    {
        [Required(ErrorMessage = "O ID é obrigatório.")]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Valor Pago é obrigatório.")]
        public decimal AmountPaid { get; set; }

        public DateTime PaidAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "paid";
    }
    public class CancelAccountPayableDTO : RequestDTO
    {
        [Required(ErrorMessage = "O ID é obrigatório.")]
        public string Id { get; set; } = string.Empty;
    }
}
