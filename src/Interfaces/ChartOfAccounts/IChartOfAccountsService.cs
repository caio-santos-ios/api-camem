using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;

namespace api_camem.src.Interfaces
{
    public interface IChartOfAccountsService
    {
        Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAsync(string id);
        Task<ResponseApi<ChartOfAccounts?>> CreateAsync(ChartOfAccounts chartOfAccounts);
        Task<ResponseApi<ChartOfAccounts?>> UpdateAsync(ChartOfAccounts chartOfAccounts);
        Task<ResponseApi<ChartOfAccounts?>> DeleteAsync(string id);
    }
}