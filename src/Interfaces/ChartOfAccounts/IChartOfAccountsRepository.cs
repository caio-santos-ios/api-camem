using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.Utils;

namespace api_camem.src.Interfaces
{
    public interface IChartOfAccountsRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<ChartOfAccounts> pagination);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<ChartOfAccounts> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<ChartOfAccounts?>> GetByIdAsync(string id);
        Task<ResponseApi<long>> GetNextCodeAsync(string type, string groupDRE);
        Task<int> GetCountDocumentsAsync(PaginationUtil<ChartOfAccounts> pagination);
        Task<ResponseApi<ChartOfAccounts?>> CreateAsync(ChartOfAccounts chartOfAccounts);
        Task<ResponseApi<ChartOfAccounts?>> UpdateAsync(ChartOfAccounts chartOfAccounts);
        Task<ResponseApi<ChartOfAccounts>> DeleteAsync(string id);
    }
}