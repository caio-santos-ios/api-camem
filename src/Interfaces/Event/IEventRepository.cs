using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;

namespace api_camem.src.Interfaces
{
    public interface IEventRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Event> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Event?>> GetByIdAsync(string id);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Event> pagination);
        Task<int> GetCountDocumentsAsync(PaginationUtil<Event> pagination);
        Task<ResponseApi<Event?>> CreateAsync(Event address);
        Task<ResponseApi<Event?>> UpdateAsync(Event address);
        Task<ResponseApi<Event>> DeleteAsync(DeleteDTO request);
    }
}
