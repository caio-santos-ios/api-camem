using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;

namespace api_camem.src.Interfaces
{
    public interface ILoggerRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Logger> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Logger?>> GetByIdAsync(string id);
        Task<int> GetCountDocumentsAsync(PaginationUtil<Logger> pagination);
        Task<ResponseApi<Logger?>> CreateAsync(Logger logger);
        Task<ResponseApi<Logger?>> UpdateAsync(Logger logger);
        Task<ResponseApi<Logger>> DeleteAsync(DeleteDTO request);
    }
}
