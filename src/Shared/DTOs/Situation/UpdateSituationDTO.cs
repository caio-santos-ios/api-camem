using api_camem.src.Models;

namespace api_camem.src.Shared.DTOs
{
    public class UpdateSituationDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name {get; set;} = string.Empty;
        public Style Style {get; set;} = new();
        public bool Start {get; set;}
        public bool End {get; set;}
        public bool Quite {get; set;}
        public bool GenerateFinancial {get; set;}
        public bool AppearsOnPanel {get; set;}
        public int Sequence {get; set;}
    }
}
