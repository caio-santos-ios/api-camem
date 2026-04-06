using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.Utils;

namespace api_camem.src.Interfaces
{
public interface ISupplierRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Supplier> pagination);
    Task<ResponseApi<List<dynamic>>> GetAutocompleteAsync(PaginationUtil<Supplier> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Supplier?>> GetByIdAsync(string id);
    Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Supplier> pagination);
    Task<ResponseApi<Supplier?>> GetByEmailAsync(string email, string id);
    Task<ResponseApi<Supplier?>> GetByDocumentAsync(string document, string id);
    Task<int> GetCountDocumentsAsync(PaginationUtil<Supplier> pagination);
    Task<ResponseApi<Supplier?>> CreateAsync(Supplier address);
    Task<ResponseApi<Supplier?>> UpdateAsync(Supplier address);
    Task<ResponseApi<Supplier>> DeleteAsync(string id);
}
}