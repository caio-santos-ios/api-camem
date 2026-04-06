using api_camem.src.Handlers;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;
using AutoMapper;

namespace api_camem.src.Services
{
    public class AccountPayableService(IAccountPayableRepository repository, CountHandler countHandler, IMapper _mapper) : IAccountPayableService
    {
        #region READ
        public async Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<AccountPayable> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> accountsPayable = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                PaginationApi<List<dynamic>> data = new(accountsPayable.Data, count, pagination.PageNumber, pagination.PageSize);
                return new(data, 200, "Contas a pagar listados com sucesso");
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
                ResponseApi<dynamic?> accountPayable = await repository.GetByIdAggregateAsync(id);
                if (accountPayable.Data is null) return new(null, 404, "Conta a pagar não encontrada");
                return new(accountPayable.Data, 200, "Conta a pagar obtido com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<AccountPayable?>> CreateAsync(CreateAccountPayableDTO request)
        {
            try
            {
                AccountPayable accountPayable = _mapper.Map<AccountPayable>(request);

                accountPayable.Code = await countHandler.NextCountAsync("account-payable");
                accountPayable.Status = "Em Aberto";
                accountPayable.AmountPaid = 0;

                ResponseApi<AccountPayable?> response = await repository.CreateAsync(accountPayable);
                if (response.Data is null) return new(null, 400, "Falha ao criar conta a pagar.");
                
                return new(response.Data, 201, "Conta a pagar criada com sucesso.");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<AccountPayable?>> UpdateAsync(UpdateAccountPayableDTO request)
        {
            try
            {
                ResponseApi<AccountPayable?> existing = await repository.GetByIdAsync(request.Id);
                if (existing.Data is null) return new(null, 404, "Conta a pagar não encontrada");

                AccountPayable accountPayable = _mapper.Map<AccountPayable>(request);
                accountPayable.UpdatedAt = DateTime.UtcNow;
                accountPayable.UpdatedBy = request.UpdatedBy;
                accountPayable.Code = existing.Data.Code;
                accountPayable.Status = existing.Data.Status;
                accountPayable.AmountPaid = existing.Data.AmountPaid;
                accountPayable.PaidAt = existing.Data.PaidAt;
                accountPayable.CreatedAt = existing.Data.CreatedAt;
                accountPayable.CreatedBy = existing.Data.CreatedBy;

                ResponseApi<AccountPayable?> response = await repository.UpdateAsync(accountPayable);
                if (!response.IsSuccess) return new(null, 400, "Falha ao atualizar conta a pagar");
                return new(response.Data, 200, "Conta a pagar atualizada com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<AccountPayable?>> PayAsync(PayAccountPayableDTO request)
        {
            try
            {
                ResponseApi<AccountPayable?> existing = await repository.GetByIdAsync(request.Id);
                if (existing.Data is null) return new(null, 404, "Conta a pagar não encontrada");
                
                if (existing.Data.Status == "Pago") return new(null, 400, "Este título já foi baixado.");

                if (request.AmountPaid <= 0) return new(null, 400, "O valor pago deve ser maior que zero.");
                if((request.AmountPaid + existing.Data.AmountPaid) > existing.Data.Amount) return new(null, 400, "O da baixa não deve ser maior que o valor a pagar.");

                existing.Data.AmountPaid += request.AmountPaid;
                existing.Data.PaidAt = request.PaidAt;
                existing.Data.UpdatedAt = DateTime.UtcNow;

                if(existing.Data.AmountPaid == existing.Data.Amount) 
                {
                    existing.Data.Status = "Pago";
                }
                else
                {
                    existing.Data.Status = "Pago Parcial";
                }

                ResponseApi<AccountPayable?> response = await repository.PayAsync(existing.Data);
                if (!response.IsSuccess) return new(null, 400, "Falha ao baixar título");

                return new(response.Data, 200, "Título baixado com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<AccountPayable?>> CancelAsync(CancelAccountPayableDTO request)
        {
            try
            {
                ResponseApi<AccountPayable?> accountPayable = await repository.GetByIdAsync(request.Id);
                if (accountPayable.Data is null) return new(null, 404, "Conta a Pagar não encontrada");

                if (accountPayable.Data.Status != "Pago" && accountPayable.Data.Status != "Pago Parcial") return new(null, 400, "Este título não pode ser cancelado.");

                accountPayable.Data.Status = "Cancelado";
                accountPayable.Data.UpdatedAt = DateTime.UtcNow;
                accountPayable.Data.UpdatedBy = request.UpdatedBy;

                ResponseApi<AccountPayable?> response = await repository.UpdateAsync(accountPayable.Data);
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
        public async Task<ResponseApi<AccountPayable>> DeleteAsync(string id)
        {
            try
            {
                ResponseApi<AccountPayable> response = await repository.DeleteAsync(id);
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
