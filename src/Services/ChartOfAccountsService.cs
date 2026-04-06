using api_camem.src.Handlers;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;

namespace api_camem.src.Services
{
    public class ChartOfAccountsService(IChartOfAccountsRepository repository, CountHandler countHandler) : IChartOfAccountsService
    {
        #region READ
        public async Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<ChartOfAccounts> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> accountsReceivable = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                PaginationApi<List<dynamic>> data = new(accountsReceivable.Data, count, pagination.PageNumber, pagination.PageSize); 
                return new(data, 200, "Planos de contas listados com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<ChartOfAccounts> pagination = new(request.QueryParams);

                ResponseApi<List<dynamic>> list = await repository.GetSelectAsync(pagination);

                return new(list.Data);
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<dynamic?>> GetByIdAsync(string id)
        {
            try
            {
                dynamic? obj = (await repository.GetByIdAggregateAsync(id)).Data;
                return new(obj);
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<ChartOfAccounts?>> CreateAsync(ChartOfAccounts chartOfAccounts)
        {
            try
            {
                chartOfAccounts.CreatedAt = DateTime.UtcNow;
                string code = await countHandler.NextCountAsync("chart-of-account");
                ResponseApi<long> codeDRE = await repository.GetNextCodeAsync(chartOfAccounts.Type, chartOfAccounts.GroupDRE);
                chartOfAccounts.Code = $"{chartOfAccounts.Account}.{codeDRE.Data.ToString().PadLeft(4, '0')}";

                ResponseApi<ChartOfAccounts?> response = await repository.CreateAsync(chartOfAccounts);
                return new(response.Data, response.StatusCode, "Conta criada com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<ChartOfAccounts?>> UpdateAsync(ChartOfAccounts chartOfAccounts)
        {
            try
            {
                ResponseApi<ChartOfAccounts?> existingAccount = await repository.GetByIdAsync(chartOfAccounts.Id);

                if (existingAccount.Data is null)
                {
                    return new(null, 404, "Conta não encontrada");
                }

                existingAccount.Data.UpdatedAt = DateTime.UtcNow;
                existingAccount.Data.UpdatedBy = chartOfAccounts.UpdatedBy;
                existingAccount.Data.Name = chartOfAccounts.Name;
                existingAccount.Data.Type = chartOfAccounts.Type;
                existingAccount.Data.Account = chartOfAccounts.Account;
                existingAccount.Data.GroupDRE = chartOfAccounts.GroupDRE;

                ResponseApi<ChartOfAccounts?> response = await repository.UpdateAsync(existingAccount.Data);
                return new(response.Data, response.StatusCode, "Conta atualizada com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<ChartOfAccounts?>> DeleteAsync(string id)
        {
            try
            {
                ResponseApi<ChartOfAccounts?> existingAccount = await repository.GetByIdAsync(id);

                if (existingAccount.Data is null) return new(null, 404, "Plano de Contas não encontrada");

                existingAccount.Data.DeletedAt = DateTime.UtcNow;

                ResponseApi<ChartOfAccounts> response = await repository.DeleteAsync(id);
                return new(response.Data, response.StatusCode, "Plano de Contas excluída com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion
    }
}