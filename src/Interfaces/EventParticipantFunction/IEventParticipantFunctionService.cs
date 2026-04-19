using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;

namespace api_camem.src.Interfaces
{
    public interface IEventParticipantFunctionService
    {
        Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request);
        Task<ResponseApi<EventParticipantFunction?>> CreateAsync(CreateEventParticipantFunctionDTO request);
        Task<ResponseApi<EventParticipantFunction?>> UpdateAsync(UpdateEventParticipantFunctionDTO request);
        Task<ResponseApi<EventParticipantFunction>> DeleteAsync(DeleteDTO request);
    }
}