using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;

namespace api_camem.src.Interfaces
{
    public interface IEventService
    {
        Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request);
        Task<ResponseApi<Event?>> CreateAsync(CreateEventDTO request);
        Task<ResponseApi<Event?>> UpdateAsync(UpdateEventDTO request);
        Task<ResponseApi<Event?>> PublishAsync(UpdateEventDTO request);
        Task<ResponseApi<Event?>> FinishAsync(UpdateEventDTO request);
        Task<ResponseApi<Event>> DeleteAsync(DeleteDTO request);
    }
}