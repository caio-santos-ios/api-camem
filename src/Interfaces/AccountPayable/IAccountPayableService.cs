using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;

namespace api_camem.src.Interfaces
{
    public interface IAccountPayableService
    {
        Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<AccountPayable?>> CreateAsync(CreateAccountPayableDTO request);
        Task<ResponseApi<AccountPayable?>> UpdateAsync(UpdateAccountPayableDTO request);
        Task<ResponseApi<AccountPayable?>> PayAsync(PayAccountPayableDTO request);
        Task<ResponseApi<AccountPayable?>> CancelAsync(CancelAccountPayableDTO request);
        Task<ResponseApi<AccountPayable>> DeleteAsync(string id);
    }
}
