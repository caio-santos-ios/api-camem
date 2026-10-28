using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;
using AutoMapper;

namespace api_camem.src.Services
{
    public class EventParticipantService
    (
        IEventParticipantRepository repository,
        IEventRepository eventRepository,
        IMapper _mapper
    ) : IEventParticipantService
    {
        #region READ
        public async Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<EventParticipant> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> events = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                PaginationApi<List<dynamic>> data = new(events.Data, count, pagination.PageNumber, pagination.PageSize);
                return new(data, 200, "Participantes listados com sucesso");
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
                ResponseApi<dynamic?> eventParticipant = await repository.GetByIdAggregateAsync(id);
                if(eventParticipant.Data is null) return new(null, 404, "Participante não encontrada");
                return new(eventParticipant.Data);
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
                PaginationUtil<EventParticipant> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> eventParticipant = await repository.GetSelectAsync(pagination);
                return new(eventParticipant.Data);
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        } 
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<EventParticipant?>> CreateAsync(CreateEventParticipantDTO request)
        {
            try
            {
                EventParticipant eventParticipant = _mapper.Map<EventParticipant>(request);
                eventParticipant.IsPresence = true;
                eventParticipant.NotesPresence = string.Empty;

                ResponseApi<EventParticipant?> response = await repository.CreateAsync(eventParticipant);

                if(response.Data is null) return new(null, 400, "Falha ao criar Participante.");

                ResponseApi<Event?> evenT = await eventRepository.GetByIdAsync(request.EventId);
                if(evenT.Data is not null && !string.IsNullOrEmpty(request.UserId))
                {
                    evenT.Data.UserIds.Add(request.UserId);
                    await eventRepository.UpdateAsync(evenT.Data);
                }

                // A função é apenas um modelo (nome/horas) e pode ser compartilhada por vários participantes.
                // O vínculo fica no próprio participante (FunctionId); o documento da função NÃO é alterado aqui
                // (antes o EventParticipantId da função era sobrescrito e só o último participante gerava certificado).

                return new(response.Data, 201, "Participante criado com sucesso.");
            }
            catch
            { 
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }

        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<EventParticipant?>> UpdateAsync(UpdateEventParticipantDTO request)
        {
            try
            {
                ResponseApi<EventParticipant?> eventResponse = await repository.GetByIdAsync(request.Id);
                if(eventResponse.Data is null) return new(null, 404, "Falha ao atualizar");
                
                EventParticipant eventParticipant = _mapper.Map<EventParticipant>(request);
                eventParticipant.UpdatedAt = DateTime.UtcNow;
                eventParticipant.CreatedAt = eventResponse.Data.CreatedAt;
                // A presença é controlada apenas pela tela de presença; a edição do participante não deve resetá-la
                eventParticipant.IsPresence = eventResponse.Data.IsPresence;
                eventParticipant.NotesPresence = eventResponse.Data.NotesPresence;

                ResponseApi<EventParticipant?> response = await repository.UpdateAsync(eventParticipant);
                if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

                ResponseApi<Event?> evenT = await eventRepository.GetByIdAsync(request.EventId);
                if(evenT.Data is not null && !string.IsNullOrEmpty(request.UserId))
                {
                    evenT.Data.UserIds.Add(request.UserId);
                    await eventRepository.UpdateAsync(evenT.Data);
                }

                // Troca de função: basta o FunctionId do participante (já gravado acima).
                // Não mexer no documento da função, pois ele pode estar em uso por outros participantes.

                return new(response.Data, 200, "Atualizado com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<EventParticipant?>> UpdatePresenceAsync(UpdatePresenceEventParticipantDTO request)
        {
            try
            {
                ResponseApi<EventParticipant?> eventResponse = await repository.GetByIdAsync(request.Id);
                if(eventResponse.Data is null) return new(null, 404, "Falha ao atualizar");

                eventResponse.Data.UpdatedAt = DateTime.UtcNow;
                // Presença gravada no participante (antes era gravada na função compartilhada, afetando todos que a usavam)
                eventResponse.Data.IsPresence = request.IsPresence;
                eventResponse.Data.NotesPresence = request.NotesPresence;

                ResponseApi<EventParticipant?> response = await repository.UpdateAsync(eventResponse.Data);
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
        public async Task<ResponseApi<EventParticipant>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                ResponseApi<EventParticipant> eventParticipant = await repository.DeleteAsync(request);
                if(!eventParticipant.IsSuccess || eventParticipant.Data is null) return new(null, 400, eventParticipant.Message);

                ResponseApi<Event?> evenT = await eventRepository.GetByIdAsync(eventParticipant.Data.EventId);
                if(evenT.Data is not null)
                {
                    // UserIds guarda o id do usuário (não o id do participante)
                    evenT.Data.UserIds = evenT.Data.UserIds.Where(x => x != eventParticipant.Data.UserId).ToList();

                    await eventRepository.UpdateAsync(evenT.Data);
                }
                
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