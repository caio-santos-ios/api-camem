using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;

namespace api_camem.src.Interfaces
{
    public interface ITemplateRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Template> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Template?>> GetByIdAsync(string id);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Template> pagination);
        Task<int> GetCountDocumentsAsync(PaginationUtil<Template> pagination);
        Task<ResponseApi<Template?>> CreateAsync(Template address);
        Task<ResponseApi<Template?>> UpdateAsync(Template address);
        Task<ResponseApi<Template>> DeleteAsync(DeleteDTO request);
    }
}
