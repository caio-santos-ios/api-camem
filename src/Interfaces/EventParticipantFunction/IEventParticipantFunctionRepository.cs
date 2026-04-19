using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;

namespace api_camem.src.Interfaces
{
    public interface IEventParticipantFunctionRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<EventParticipantFunction> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<EventParticipantFunction?>> GetByIdAsync(string id);
        Task<ResponseApi<List<EventParticipantFunction>>> GetByEventParticipantIdAsync(string eventParticipantId);
        Task<ResponseApi<List<EventParticipantFunction>>> GetAllByEventIdAsync(string eventId);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<EventParticipantFunction> pagination);
        Task<int> GetCountDocumentsAsync(PaginationUtil<EventParticipantFunction> pagination);
        Task<ResponseApi<EventParticipantFunction?>> CreateAsync(EventParticipantFunction address);
        Task<ResponseApi<EventParticipantFunction?>> UpdateAsync(EventParticipantFunction address);
        Task<ResponseApi<EventParticipantFunction>> DeleteAsync(DeleteDTO request);
    }
}
