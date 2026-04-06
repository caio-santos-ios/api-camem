using api_camem.src.Interfaces;
using api_camem.src.Models.Base;

namespace api_camem.src.Services
{
    public class DreService(IDreRepository repository) : IDreService
    {
        public async Task<ResponseApi<dynamic?>> GenerateAsync(DateTime startDate, DateTime endDate, string regime)
        {
            try
            {
                if (startDate > endDate)
                {
                    return new(null, 400, "Data inicial não pode ser maior que data final");
                }

                if (regime != "caixa" && regime != "competencia")
                {
                    return new(null, 400, "Regime deve ser 'caixa' ou 'competencia'");
                }

                ResponseApi<dynamic> response = await repository.GenerateAsync(startDate, endDate, regime);
                
                return new(response.Data, 200, "DRE gerado com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
    }
}