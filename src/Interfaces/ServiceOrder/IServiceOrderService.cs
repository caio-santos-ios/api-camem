using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;

namespace api_camem.src.Interfaces
{
    public interface IServiceOrderService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<dynamic?>> CheckWarrantyAsync(string? customerId, string? serialImei);
        Task<ResponseApi<ServiceOrder?>> CreateAsync(CreateServiceOrderDTO request);
        Task<ResponseApi<ServiceOrder?>> UpdateAsync(UpdateServiceOrderDTO request);
        Task<ResponseApi<ServiceOrder?>> UpdateStatusAsync(UpdateStatusServiceOrderDTO request);
        Task<ResponseApi<ServiceOrder?>> CloseAsync(CloseServiceOrderDTO request);
        Task<ResponseApi<ServiceOrder>> DeleteAsync(string id);
    }
}