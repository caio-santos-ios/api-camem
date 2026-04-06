using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.Utils;

namespace api_camem.src.Interfaces
{
public interface IServiceOrderRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<ServiceOrder> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<ServiceOrder?>> GetByIdAsync(string id);
    Task<int> GetCountDocumentsAsync(PaginationUtil<ServiceOrder> pagination);
    Task<ResponseApi<dynamic?>> CheckWarrantyAsync(string? customerId, string? serialImei);
    Task<ResponseApi<ServiceOrder?>> CreateAsync(ServiceOrder serviceOrder);
    Task<ResponseApi<ServiceOrder?>> UpdateAsync(ServiceOrder serviceOrder);
    Task<ResponseApi<ServiceOrder>> DeleteAsync(string id);
}
}
