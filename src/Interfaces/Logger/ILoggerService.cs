using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;

namespace api_camem.src.Interfaces
{
    public interface ILoggerService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Logger?>> CreateAsync(CreateLoggerDTO request);
        Task<ResponseApi<Logger?>> UpdateAsync(UpdateLoggerDTO request);
        Task<ResponseApi<Logger>> DeleteAsync(DeleteDTO request);
    }
}