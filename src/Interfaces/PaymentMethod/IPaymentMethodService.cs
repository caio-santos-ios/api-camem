using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;

namespace api_camem.src.Interfaces
{
    public interface IPaymentMethodService
    {
        Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request);
        Task<ResponseApi<PaymentMethod?>> CreateAsync(CreatePaymentMethodDTO request);
        Task<ResponseApi<PaymentMethod?>> UpdateAsync(UpdatePaymentMethodDTO request);
        Task<ResponseApi<PaymentMethod>> DeleteAsync(string id);
    }
}