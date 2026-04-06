using System.ComponentModel.DataAnnotations;

namespace api_camem.src.Shared.DTOs
{
    public class CreateTriggerDTO : RequestDTO
    {
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "O E-mail é obrigatório.")]
        public string Email { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "O intervalo deve ser maior que zero.")]
        public int IntervalValue { get; set; } = 1;

        /// <summary>minutes | hours | days</summary>
        public string IntervalUnit { get; set; } = "hours";
    }
}