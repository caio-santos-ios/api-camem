using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;

namespace api_camem.src.Interfaces
{
    public interface ICustomCertificateService
    {
        Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<dynamic?>> GetLastAsync();
        Task<ResponseApi<dynamic?>> GetValidateKeyAsync(string keyCertificate);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request);
        Task<ResponseApi<CustomCertificate?>> CreateAsync(CreateCustomCertificateDTO request);
        Task<ResponseApi<CustomCertificate?>> UpdateAsync(UpdateCustomCertificateDTO request);
        Task<ResponseApi<string?>> UploadSignature(UploadSignatureCustomCertificateDTO request);
        Task<ResponseApi<CustomCertificate>> DeleteAsync(string id);
    }
}