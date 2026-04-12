using api_camem.src.Handlers;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Templates;
using api_camem.src.Shared.Utils;
using AutoMapper;

namespace api_camem.src.Services
{
    public class EventService
    (
        IEventRepository repository,
        IEventParticipantRepository eventParticipantRepository,
        IUserRepository userRepository,
        MailHandler mailHandler,
        MailTemplate mailTemplate,
        INotificationService notificationService,
        ICertificateService certificateService,
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
        public async Task<ResponseApi<Event?>> PublishAsync(UpdateEventDTO request)
        {
            try
            {
                ResponseApi<Event?> eventResponse = await repository.GetByIdAsync(request.Id);
                if(eventResponse.Data is null) return new(null, 404, "Falha ao atualizar");
                
                eventResponse.Data.UpdatedAt = DateTime.UtcNow;
                eventResponse.Data.Status = "Publicado";
                eventResponse.Data.KeyCertificate = Guid.NewGuid().ToString("N");

                ResponseApi<List<EventParticipant>> eventParticipants = await eventParticipantRepository.GetAllByEventIdAsync(request.Id);
                if(eventParticipants.Data is null) return new(null, 400, "O evento precisa ter pelo menos 1 participante");
                if(eventParticipants.Data.Count == 0) return new(null, 400, "O evento precisa ter pelo menos 1 participante");

                ResponseApi<Event?> response = await repository.UpdateAsync(eventResponse.Data);
                if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
                
                foreach (EventParticipant eventParticipant in eventParticipants.Data)
                {
                    if(!string.IsNullOrEmpty(eventParticipant.UserId))
                    {
                        ResponseApi<User?> user = await userRepository.GetByIdAsync(eventParticipant.UserId);
                        if(user.Data is not null)
                        {
                            List<string> functions = eventParticipant.Functions.Select(x => x.Name).ToList();
                            decimal hours = eventParticipant.Functions.Sum(x => x.Hours);
                            string functionName = "";
                            if(functions.Count > 1)
                            {
                                functionName = $"{functions.Count} funções";
                            }
                            else
                            {
                                functionName = functions.Count == 0 ? "Sem função" : functions[0];
                            }

                            string endDate = "";
                            if(eventResponse.Data.EndDate is not null)
                            {
                                endDate = eventResponse.Data.EndDate?.ToString("dd/MM/yyyy") ?? "";
                            }

                            await mailHandler.SendMailAsync(user.Data.Email, "Novo Evento", await mailTemplate.EventPublish(user.Data.Name, eventResponse.Data.Title, eventResponse.Data.StartDate.ToString("dd/MM/yyyy"), endDate, functionName, hours.ToString()));
                        }
                    }
                }

                List<string> userIds = eventParticipants.Data.Select(u => u.UserId).ToList();
                if (userIds.Count > 0)
                {
                    await notificationService.SendToManyAsync(userIds, new()
                    {
                        Link      = $"/events",
                        Title     = "Novo evento",
                        Message   = $"Você foi adicionado em um novo evento.",
                        Type      = "Notificacao",
                        CreatedBy = request.CreatedBy
                    });
                }

                return new(response.Data, 200, "Evento Finalizado com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<Event?>> FinishAsync(UpdateEventDTO request)
        {
            try
            {
                ResponseApi<Event?> eventResponse = await repository.GetByIdAsync(request.Id);
                if(eventResponse.Data is null) return new(null, 404, "Falha ao atualizar");
                
                eventResponse.Data.UpdatedAt = DateTime.UtcNow;
                eventResponse.Data.Status = "Finalizado";

                ResponseApi<List<EventParticipant>> eventParticipants = await eventParticipantRepository.GetAllByEventIdAsync(request.Id);

                ResponseApi<Event?> response = await repository.UpdateAsync(eventResponse.Data);
                if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
                
                List<string> userIds = [];
                if(eventParticipants.Data is not null)
                {
                    foreach (EventParticipant eventParticipant in eventParticipants.Data)
                    {
                        if(!string.IsNullOrEmpty(eventParticipant.UserId))
                        {
                            ResponseApi<User?> user = await userRepository.GetByIdAsync(eventParticipant.UserId);
                            if(user.Data is not null)
                            {
                                if(eventParticipant.Functions.Where(x => x.IsPresence).Any())
                                {
                                    userIds.Add(user.Data.Id);
                                    List<string> functions = eventParticipant.Functions.Where(x => x.IsPresence).Select(x => x.Name).ToList();
                                    decimal hours = eventParticipant.Functions.Where(x => x.IsPresence).Sum(x => x.Hours);
                                    string functionName = "";
                                    if(functions.Count > 1)
                                    {
                                        functionName = $"{functions.Count} funções";
                                    }
                                    else
                                    {
                                        functionName = functions.Count == 0 ? "Sem função" : functions[0];
                                    }

                                    string endDate = "";
                                    if(eventResponse.Data.EndDate is not null)
                                    {
                                        endDate = eventResponse.Data.EndDate?.ToString("dd/MM/yyyy") ?? "";
                                    }

                                    await mailHandler.SendMailAsync(user.Data.Email, "Novo Certificado", await mailTemplate.EventFinish(user.Data.Name, eventResponse.Data.Title, eventResponse.Data.StartDate.ToString("dd/MM/yyyy"), endDate, functionName, hours.ToString()));
                                    var res = await certificateService.CreateAsync(new ()
                                    {
                                        Name = eventResponse.Data.Title,
                                        EventId = eventResponse.Data.Id,
                                        UserId = eventParticipant.UserId,
                                        Functions = functions,
                                        Hours = hours,
                                        KeyCertificate = eventResponse.Data.KeyCertificate,
                                        CreatedBy = request.CreatedBy
                                    });
                                }
                            }
                        }
                    }
                }

                if (userIds.Count > 0)
                {
                    await notificationService.SendToManyAsync(userIds, new()
                    {
                        Link      = $"/my-certificates",
                        Title     = "Novo Certificado",
                        Message   = $"Você tem um novo certificado.",
                        Type      = "Notificacao",
                        CreatedBy = request.CreatedBy
                    });
                }

                return new(response.Data, 200, "Evento finalizado com sucesso");
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