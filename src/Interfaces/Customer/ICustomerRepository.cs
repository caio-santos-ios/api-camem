using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.Utils;

namespace api_camem.src.Interfaces
{
public interface ICustomerRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Customer> pagination);
    Task<ResponseApi<List<dynamic>>> GetMovementAsync(PaginationUtil<Customer> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Customer?>> GetByIdAsync(string id);
    Task<ResponseApi<Customer?>> GetByEmailAsync(string email, string id);
    Task<ResponseApi<Customer?>> GetByDocumentAsync(string document, string id);
    Task<int> GetCountDocumentsAsync(PaginationUtil<Customer> pagination);
    Task<ResponseApi<Customer?>> CreateAsync(Customer address);
    Task<ResponseApi<Customer?>> UpdateAsync(Customer address);
    Task<ResponseApi<Customer>> DeleteAsync(string id);
}
}
