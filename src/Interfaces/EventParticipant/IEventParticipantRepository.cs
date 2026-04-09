using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;

namespace api_camem.src.Interfaces
{
    public interface IEventParticipantRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<EventParticipant> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<EventParticipant?>> GetByIdAsync(string id);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<EventParticipant> pagination);
        Task<ResponseApi<EventParticipant?>> GetByUserIdAsync(string userId, string id);
        Task<int> GetCountDocumentsAsync(PaginationUtil<EventParticipant> pagination);
        Task<ResponseApi<EventParticipant?>> CreateAsync(EventParticipant address);
        Task<ResponseApi<EventParticipant?>> UpdateAsync(EventParticipant address);
        Task<ResponseApi<EventParticipant>> DeleteAsync(DeleteDTO request);
    }
}
