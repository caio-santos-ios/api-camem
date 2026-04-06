using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.Utils;

namespace api_camem.src.Interfaces
{
public interface ISituationRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Situation> pagination);
    Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Situation> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Situation?>> GetByIdAsync(string id);
    Task<ResponseApi<List<Situation>>> GetByAppearsOnPanelAsync();
    Task<ResponseApi<Situation?>> GetByMomentAsync(string moment);
    Task<int> GetCountDocumentsAsync(PaginationUtil<Situation> pagination);
    Task<ResponseApi<Situation?>> CreateAsync(Situation situation);
    Task<ResponseApi<Situation?>> UpdateAsync(Situation situation);
    Task<ResponseApi<Situation>> DeleteAsync(string id);
}
}
