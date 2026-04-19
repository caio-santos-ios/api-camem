using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;
using AutoMapper;

namespace api_camem.src.Services
{
    public class EventParticipantFunctionService
    (
        IEventParticipantFunctionRepository repository,
        IMapper _mapper
    ) : IEventParticipantFunctionService
    {
        #region READ
        public async Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<EventParticipantFunction> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> eventParticipantFunctions = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                PaginationApi<List<dynamic>> data = new(eventParticipantFunctions.Data, count, pagination.PageNumber, pagination.PageSize);
                return new(data, 200, "Funções listados com sucesso");
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
                ResponseApi<dynamic?> eventParticipantFunction = await repository.GetByIdAggregateAsync(id);
                if(eventParticipantFunction.Data is null) return new(null, 404, "Função não encontrada");
                return new(eventParticipantFunction.Data);
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
                PaginationUtil<EventParticipantFunction> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> eventParticipantFunction = await repository.GetSelectAsync(pagination);
                return new(eventParticipantFunction.Data);
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        } 
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<EventParticipantFunction?>> CreateAsync(CreateEventParticipantFunctionDTO request)
        {
            try
            {
                EventParticipantFunction eventParticipantFunction = _mapper.Map<EventParticipantFunction>(request);

                ResponseApi<EventParticipantFunction?> response = await repository.CreateAsync(eventParticipantFunction);

                if(response.Data is null) return new(null, 400, "Falha ao criar Função.");

                return new(response.Data, 201, "Função criado com sucesso.");
            }
            catch
            { 
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }

        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<EventParticipantFunction?>> UpdateAsync(UpdateEventParticipantFunctionDTO request)
        {
            try
            {
                ResponseApi<EventParticipantFunction?> eventResponse = await repository.GetByIdAsync(request.Id);
                if(eventResponse.Data is null) return new(null, 404, "Falha ao atualizar");

                EventParticipantFunction eventParticipantFunction = _mapper.Map<EventParticipantFunction>(request);
                eventParticipantFunction.UpdatedAt = DateTime.UtcNow;
                eventParticipantFunction.CreatedAt = eventResponse.Data.CreatedAt;

                ResponseApi<EventParticipantFunction?> response = await repository.UpdateAsync(eventParticipantFunction);
                if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

                return new(response.Data, 200, "Atualizado com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<EventParticipantFunction>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                ResponseApi<EventParticipantFunction> eventParticipantFunction = await repository.DeleteAsync(request);
                if(!eventParticipantFunction.IsSuccess || eventParticipantFunction.Data is null) return new(null, 400, eventParticipantFunction.Message);
                
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