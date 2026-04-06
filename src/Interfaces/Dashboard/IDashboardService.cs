using api_camem.src.Models.Base;

namespace api_camem.src.Interfaces
{
    public interface IDashboardService
    {
        Task<ResponseApi<dynamic>> GetAccountReceivableCard(DateTime startDate, DateTime endDate);
        Task<ResponseApi<dynamic>> GetAccountPayableCard(DateTime startDate, DateTime endDate);
        Task<ResponseApi<dynamic>> GetCashFlowCard(DateTime startDate, DateTime endDate);
        Task<ResponseApi<dynamic>> GetEntrieExitBar(DateTime startDate, DateTime endDate);
        Task<ResponseApi<dynamic>> GetTopRevenueBar(DateTime startDate, DateTime endDate);
        Task<ResponseApi<dynamic>> GetExpenseCategoryPie(DateTime startDate, DateTime endDate);
        Task<ResponseApi<dynamic>> GetEvolutionBalanceArea(DateTime startDate, DateTime endDate);
    }
}
