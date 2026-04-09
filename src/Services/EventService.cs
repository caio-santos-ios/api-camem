using api_camem.src.Handlers;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;
using AutoMapper;

namespace api_camem.src.Services
{
    public class EventService
    (
        IEventRepository repository,
        IMapper _mapper
    ) : IEventService
    {
        #region READ
        public async Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<Event> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> events = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                PaginationApi<List<dynamic>> data = new(events.Data, count, pagination.PageNumber, pagination.PageSize);
                return new(data, 200, "Evento listados com sucesso");
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
                ResponseApi<dynamic?> Event = await repository.GetByIdAggregateAsync(id);
                if(Event.Data is null) return new(null, 404, "Evento não encontrada");
                return new(Event.Data);
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
                PaginationUtil<Event> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> evenT = await repository.GetSelectAsync(pagination);
                return new(evenT.Data);
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        } 
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<Event?>> CreateAsync(CreateEventDTO request)
        {
            try
            {
                Event evenT = _mapper.Map<Event>(request);
                evenT.Status = "Rascunho";

                ResponseApi<Event?> response = await repository.CreateAsync(evenT);

                if(response.Data is null) return new(null, 400, "Falha ao criar Event.");
                return new(response.Data, 201, "Evento criada com sucesso.");
            }
            catch
            { 
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }

        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<Event?>> UpdateAsync(UpdateEventDTO request)
        {
            try
            {
                ResponseApi<Event?> eventResponse = await repository.GetByIdAsync(request.Id);
                if(eventResponse.Data is null) return new(null, 404, "Falha ao atualizar");
                
                Event evenT = _mapper.Map<Event>(request);
                evenT.UpdatedAt = DateTime.UtcNow;
                evenT.CreatedAt = eventResponse.Data.CreatedAt;

                ResponseApi<Event?> response = await repository.UpdateAsync(evenT);
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
        public async Task<ResponseApi<Event>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                ResponseApi<Event> evenT = await repository.DeleteAsync(request);
                if(!evenT.IsSuccess) return new(null, 400, evenT.Message);
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