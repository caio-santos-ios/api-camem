using api_camem.src.Interfaces;
using api_camem.src.Models.Base;

namespace api_camem.src.Services
{
    public class DashboardService(IDashboardRepository repository) : IDashboardService
    {
        #region FINANCIAL CARDS
        public async Task<ResponseApi<dynamic>> GetAccountReceivableCard(DateTime startDate, DateTime endDate)
        {
            try
            {
                ResponseApi<dynamic> dash = await repository.GetAccountReceivableCard(startDate, endDate);

                return new(dash.Data, 200, "Contas a receber listados com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<dynamic>> GetAccountPayableCard(DateTime startDate, DateTime endDate)
        {
            try
            {
                ResponseApi<dynamic> dash = await repository.GetAccountPayableCard(startDate, endDate);

                return new(dash.Data, 200, "Contas a pagar listados com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<dynamic>> GetCashFlowCard(DateTime startDate, DateTime endDate)
        {
            try
            {
                ResponseApi<dynamic> dash = await repository.GetCashFlowCard(startDate, endDate);

                return new(dash.Data, 200, "Fluxo de Caixa listados com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion
        #region FINANCIAL BAR
        public async Task<ResponseApi<dynamic>> GetEntrieExitBar(DateTime startDate, DateTime endDate)
        {
            try
            {
                ResponseApi<dynamic> dash = await repository.GetEntrieExitBar(startDate, endDate);

                return new(dash.Data, 200, "Entradas VS Saidas listados com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<dynamic>> GetTopRevenueBar(DateTime startDate, DateTime endDate)
        {
            try
            {
                ResponseApi<dynamic> dash = await repository.GetTopRevenueBar(startDate, endDate);

                return new(dash.Data, 200, "Top receitas listados com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion
        #region FINANCIAL PIE
        public async Task<ResponseApi<dynamic>> GetExpenseCategoryPie(DateTime startDate, DateTime endDate)
        {
            try
            {
                ResponseApi<dynamic> dash = await repository.GetExpenseCategoryPie(startDate, endDate);

                return new(dash.Data, 200, "Despesas por categoria listados com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion
        #region FINANCIAL AREA
        public async Task<ResponseApi<dynamic>> GetEvolutionBalanceArea(DateTime startDate, DateTime endDate)
        {
            try
            {
                ResponseApi<dynamic> dash = await repository.GetEvolutionBalanceArea(startDate, endDate);

                return new(dash.Data, 200, "Evolução do saldo listados com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion
    }
}
