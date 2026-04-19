using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.Utils;

namespace api_camem.src.Interfaces
{
    public interface ICustomCertificateRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<CustomCertificate> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<dynamic?>> GetValidateKeyAsync(string keyCustomCertificate);
        Task<ResponseApi<CustomCertificate?>> GetByIdAsync(string id);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<CustomCertificate> pagination);
        Task<int> GetCountDocumentsAsync(PaginationUtil<CustomCertificate> pagination);
        Task<ResponseApi<CustomCertificate?>> CreateAsync(CustomCertificate address);
        Task<ResponseApi<CustomCertificate?>> UpdateAsync(CustomCertificate address);
        Task<ResponseApi<CustomCertificate>> DeleteAsync(string id);
    }
}
