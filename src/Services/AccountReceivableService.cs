using api_camem.src.Handlers;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;
using AutoMapper;

namespace api_camem.src.Services
{
    public class AccountReceivableService(IAccountReceivableRepository repository, CountHandler countHandler, IMapper _mapper) : IAccountReceivableService
    {
        #region READ
        public async Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<AccountReceivable> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> accountsReceivable = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                PaginationApi<List<dynamic>> data = new(accountsReceivable.Data, count, pagination.PageNumber, pagination.PageSize); 
                return new(data, 200, "Contas a receber listados com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }

        public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
        {
            try
            {
                ResponseApi<dynamic?> accountReceivable = await repository.GetByIdAggregateAsync(id);
                if (accountReceivable.Data is null) return new(null, 404, "Conta a receber não encontrada");
                return new(accountReceivable.Data, 200, "Conta a receber obtida com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<AccountReceivable?>> CreateAsync(CreateAccountReceivableDTO request)
        {
            try
            {
                AccountReceivable accountReceivable = _mapper.Map<AccountReceivable>(request);

                accountReceivable.Code = await countHandler.NextCountAsync("account-receivable");
                accountReceivable.Status = "Em Aberto";
                accountReceivable.AmountPaid = 0;

                if(request.IsPaymented)
                {
                    accountReceivable.AmountPaid = request.Amount;
                    accountReceivable.PaidAt = DateTime.UtcNow;
                    accountReceivable.Status = "Recebido";
                }

                ResponseApi<AccountReceivable?> response = await repository.CreateAsync(accountReceivable);
                if (response.Data is null) return new(null, 400, "Falha ao criar conta a receber.");
                return new(response.Data, 201, "Conta a receber criada com sucesso.");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<AccountReceivable?>> UpdateAsync(UpdateAccountReceivableDTO request)
        {
            try
            {
                ResponseApi<AccountReceivable?> existing = await repository.GetByIdAsync(request.Id);
                if (existing.Data is null) return new(null, 404, "Conta a receber não encontrada");

                AccountReceivable accountReceivable = _mapper.Map<AccountReceivable>(request);
                accountReceivable.UpdatedAt = DateTime.UtcNow;
                accountReceivable.UpdatedBy = request.UpdatedBy;

                accountReceivable.Code = existing.Data.Code;
                accountReceivable.Status = existing.Data.Status;
                accountReceivable.AmountPaid = existing.Data.AmountPaid;
                accountReceivable.PaidAt = existing.Data.PaidAt;
                accountReceivable.CreatedAt = existing.Data.CreatedAt;
                accountReceivable.CreatedBy = existing.Data.CreatedBy;

                ResponseApi<AccountReceivable?> response = await repository.UpdateAsync(accountReceivable);
                if (!response.IsSuccess) return new(null, 400, "Falha ao atualizar conta a receber");
                return new(response.Data, 200, "Conta a receber atualizada com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<AccountReceivable?>> PayAsync(PayAccountReceivableDTO request)
        {
            try
            {
                ResponseApi<AccountReceivable?> existing = await repository.GetByIdAsync(request.Id);
                if (existing.Data is null) return new(null, 404, "Conta a receber não encontrada");

                if (existing.Data.Status == "Recebido") return new(null, 400, "Este título já foi baixado.");

                if (request.AmountPaid <= 0) return new(null, 400, "O valor recebido deve ser maior que zero.");
                if((request.AmountPaid + existing.Data.AmountPaid) > existing.Data.Amount) return new(null, 400, "O da baixa não deve ser maior que o valor a receber.");

                existing.Data.AmountPaid += request.AmountPaid;
                existing.Data.PaidAt = request.PaidAt;
                existing.Data.UpdatedAt = DateTime.UtcNow;

                if(existing.Data.AmountPaid == existing.Data.Amount) 
                {
                    existing.Data.Status = "Recebido";
                }
                else
                {
                    existing.Data.Status = "Recebido Parcial";
                }

                ResponseApi<AccountReceivable?> response = await repository.PayAsync(existing.Data);
                if (!response.IsSuccess) return new(null, 400, "Falha ao baixar título");
                return new(response.Data, 200, "Título baixado com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<AccountReceivable?>> CancelAsync(CancelAccountReceivableDTO request)
        {
            try
            {
                ResponseApi<AccountReceivable?> accountReceivable = await repository.GetByIdAsync(request.Id);
                if (accountReceivable.Data is null) return new(null, 404, "Conta a receber não encontrada");

                if (accountReceivable.Data.Status != "Recebido" && accountReceivable.Data.Status != "Recebido Parcial") return new(null, 400, "Este título não pode ser cancelado.");

                accountReceivable.Data.Status = "Cancelado";
                accountReceivable.Data.UpdatedAt = DateTime.UtcNow;
                accountReceivable.Data.UpdatedBy = request.UpdatedBy;

                ResponseApi<AccountReceivable?> response = await repository.UpdateAsync(accountReceivable.Data);
                if (!response.IsSuccess) return new(null, 400, "Falha ao cancelar título");

                return new(response.Data, 200, "Título cancelado com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<AccountReceivable>> DeleteAsync(string id)
        {
            try
            {
                ResponseApi<AccountReceivable> response = await repository.DeleteAsync(id);
                if (!response.IsSuccess) return new(null, 400, response.Message);
                return new(null, 204, "Excluída com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion
    }
}
