using api_camem.src.Handlers;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;
using api_camem.src.Shared.Validators;
using AutoMapper;

namespace api_camem.src.Services
{
    public class TriggerService(
        ITriggerRepository repository,
        CountHandler countHandler,
        IMapper mapper
    ) : ITriggerService
    {
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<Trigger> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> triggers = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                return new(triggers.Data, count, pagination.PageNumber, pagination.PageSize);
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
                ResponseApi<dynamic?> trigger = await repository.GetByIdAggregateAsync(id);
                if (trigger.Data is null) return new(null, 404, "Trigger não encontrada.");
                return new(trigger.Data);
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
                PaginationUtil<Trigger> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> triggers = await repository.GetSelectAsync(pagination);
                return new(triggers.Data);
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<Trigger?>> CreateAsync(CreateTriggerDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name)) return new(null, 400, "O Nome é obrigatório.");

                if (string.IsNullOrWhiteSpace(request.Email) || !Validator.IsEmail(request.Email)) return new(null, 400, "E-mail inválido.");

                if (request.IntervalValue <= 0) return new(null, 400, "O intervalo deve ser maior que zero.");

                Trigger trigger = mapper.Map<Trigger>(request);
                trigger.Code = await countHandler.NextCountAsync("triggers");
                trigger.NextFireAt = CalculateNextFire(DateTime.UtcNow, request.IntervalValue, request.IntervalUnit);

                ResponseApi<Trigger?> response = await repository.CreateAsync(trigger);
                if (response.Data is null) return new(null, 400, "Falha ao criar Trigger.");

                return new(response.Data, 201, "Trigger criada com sucesso.");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<Trigger?>> UpdateAsync(UpdateTriggerDTO request)
        {
            try
            {
                ResponseApi<Trigger?> existing = await repository.GetByIdAsync(request.Id);
                if (existing.Data is null) return new(null, 404, "Trigger não encontrada.");
    
                if (string.IsNullOrWhiteSpace(request.Name)) return new(null, 400, "O Nome é obrigatório.");

                if (string.IsNullOrWhiteSpace(request.Email) || !Validator.IsEmail(request.Email)) return new(null, 400, "E-mail inválido.");

                if (request.IntervalValue <= 0) return new(null, 400, "O intervalo deve ser maior que zero.");

                Trigger trigger = mapper.Map<Trigger>(request);
                trigger.UpdatedAt = DateTime.UtcNow;
                trigger.CreatedAt = existing.Data.CreatedAt;
                trigger.Code = existing.Data.Code;
                trigger.LastFiredAt = existing.Data.LastFiredAt;
                trigger.NextFireAt = CalculateNextFire(DateTime.UtcNow, request.IntervalValue, request.IntervalUnit);

                ResponseApi<Trigger?> response = await repository.UpdateAsync(trigger);
                if (!response.IsSuccess) return new(null, 400, "Falha ao atualizar Trigger.");

                return new(response.Data, 200, "Trigger atualizada com sucesso.");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<Trigger>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                ResponseApi<Trigger> trigger = await repository.DeleteAsync(request);
                if (!trigger.IsSuccess) return new(null, 400, trigger.Message);
                return new(null, 204, "Trigger excluída com sucesso.");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion

        private static DateTime CalculateNextFire(DateTime from, int value, string unit) => unit switch
        {
            "minutes" => from.AddMinutes(value),
            "hours"   => from.AddHours(value),
            "days"    => from.AddDays(value),
            _         => from.AddHours(value),
        };
    }
}