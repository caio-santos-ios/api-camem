using api_camem.src.Models.Base;

namespace api_camem.src.Interfaces
{
    public interface IDreService
    {
        Task<ResponseApi<dynamic?>> GenerateAsync(DateTime startDate, DateTime endDate, string regime);
    }
}