using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;

namespace api_camem.src.Interfaces
{
    public interface IEventParticipantService
    {
        Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request);
        Task<ResponseApi<EventParticipant?>> CreateAsync(CreateEventParticipantDTO request);
        Task<ResponseApi<EventParticipant?>> UpdateAsync(UpdateEventParticipantDTO request);
        Task<ResponseApi<EventParticipant?>> UpdatePresenceAsync(UpdatePresenceEventParticipantDTO request);
        Task<ResponseApi<EventParticipant>> DeleteAsync(DeleteDTO request);
    }
}