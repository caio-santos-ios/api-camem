using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;

namespace api_camem.src.Interfaces
{
    public interface ICertificateService
    {
        Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request);
        Task<ResponseApi<Certificate?>> CreateAsync(CreateCertificateDTO request);
        Task<ResponseApi<Certificate?>> UpdateAsync(UpdateCertificateDTO request);
        Task<ResponseApi<Certificate>> DeleteAsync(string id);
    }
}