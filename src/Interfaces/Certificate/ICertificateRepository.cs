using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.Utils;

namespace api_camem.src.Interfaces
{
    public interface ICertificateRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Certificate> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<dynamic?>> GetValidateKeyAsync(string keyCertificate);
        Task<ResponseApi<Certificate?>> GetByIdAsync(string id);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Certificate> pagination);
        Task<int> GetCountDocumentsAsync(PaginationUtil<Certificate> pagination);
        Task<ResponseApi<Certificate?>> CreateAsync(Certificate address);
        Task<ResponseApi<Certificate?>> UpdateAsync(Certificate address);
        Task<ResponseApi<Certificate>> DeleteAsync(string id);
    }
}
