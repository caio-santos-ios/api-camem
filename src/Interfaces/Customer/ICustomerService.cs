using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;

namespace api_camem.src.Interfaces
{
    public interface ICustomerService
    {
        Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<List<dynamic>>> GetMovementAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Customer?>> CreateAsync(CreateCustomerDTO request);
        Task<ResponseApi<Customer?>> CreateMinimalAsync(CreateCustomerMinimalDTO request);
        Task<ResponseApi<Customer?>> UpdateAsync(UpdateCustomerDTO request);
        Task<ResponseApi<Customer?>> UpdateMinimalAsync(UpdateCustomerMinimalDTO request);
        Task<ResponseApi<Customer>> DeleteAsync(string id);
    }
}