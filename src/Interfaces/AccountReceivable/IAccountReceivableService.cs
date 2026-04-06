using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;

namespace api_camem.src.Interfaces
{
    public interface IAccountReceivableService
    {
        Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<AccountReceivable?>> CreateAsync(CreateAccountReceivableDTO request);
        Task<ResponseApi<AccountReceivable?>> UpdateAsync(UpdateAccountReceivableDTO request);
        Task<ResponseApi<AccountReceivable?>> PayAsync(PayAccountReceivableDTO request);
        Task<ResponseApi<AccountReceivable?>> CancelAsync(CancelAccountReceivableDTO request);
        Task<ResponseApi<AccountReceivable>> DeleteAsync(string id);
    }
}
