using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.Utils;

namespace api_camem.src.Interfaces
{
    public interface IAccountPayableRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<AccountPayable> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<AccountPayable?>> GetByIdAsync(string id);
        Task<int> GetCountDocumentsAsync(PaginationUtil<AccountPayable> pagination);
        Task<ResponseApi<AccountPayable?>> CreateAsync(AccountPayable accountPayable);
        Task<ResponseApi<AccountPayable?>> UpdateAsync(AccountPayable accountPayable);
        Task<ResponseApi<AccountPayable?>> PayAsync(AccountPayable accountPayable);
        Task<ResponseApi<AccountPayable>> DeleteAsync(string id);
    }
}
