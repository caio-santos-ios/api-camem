using api_camem.src.Configuration;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;


namespace api_camem.src.Repository
{
    public class EventRepository(AppDbContext context) : IEventRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Event> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    new("$skip", pagination.Skip),
                    new("$limit", pagination.Limit),
                    
                    MongoUtil.Lookup("event_participants", ["$_id"], ["$eventId"], "_event_participants", [["deleted", false]]),

                    new("$addFields", new BsonDocument
                    {
                        {"id", new BsonDocument("$toString", "$_id")},
                    }),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0}, 
                        {"id", 1},
                        {"title", 1},
                        {"description", 1},
                        {"status", 1},
                        {"startDate", 1},
                        {"endDate", 1},
                        {"userIds", 1},
                        {"participants", new BsonDocument("$map", new BsonDocument 
                            {
                                {"input", "$_event_participants"},
                                {"as", "e"},
                                {"in", new BsonDocument 
                                    {
                                        {"id", new BsonDocument("$toString", "$$e._id")},
                                        {"name", "$$e.name"},
                                        {"functions", "$$e.functions"},
                                        {"userId", "$$e.userId"},
                                    }
                                }
                            })
                        }
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.Events.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        } 
        public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
        {
            try
            {
                BsonDocument[] pipeline = [
                    new("$match", new BsonDocument{
                        {"_id", new ObjectId(id)},
                        {"deleted", false}
                    }),

                    new("$addFields", new BsonDocument
                    {
                        {"id", new BsonDocument("$toString", "$_id")},
                    }),

                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                    }),
                ];

                BsonDocument? response = await context.Events.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Event não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }   
        public async Task<ResponseApi<Event?>> GetByIdAsync(string id)
        {
            try
            {
                Event? evenT = await context.Events.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(evenT);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        } 
        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Event> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$sort", pagination.PipelineSort),
                    new("$addFields", new BsonDocument
                    {
                        {"id", new BsonDocument("$toString", "$_id")},
                    }),
                    new("$match", pagination.PipelineFilter),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"id", 1}, 
                        {"code", 1}, 
                        {"name", 1} 
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.Events.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<int> GetCountDocumentsAsync(PaginationUtil<Event> pagination)
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                new("$addFields", new BsonDocument
                {
                    {"id", new BsonDocument("$toString", "$_id")},
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0},
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.Events.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<Event?>> CreateAsync(Event evenT)
        {
            try
            {
                await context.Events.InsertOneAsync(evenT);

                return new(evenT, 201, "Evento criada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<Event?>> UpdateAsync(Event evenT)
        {
            try
            {
                await context.Events.ReplaceOneAsync(x => x.Id == evenT.Id, evenT);

                return new(evenT, 201, "Evento atualizada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<Event>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                Event? evenT = await context.Events.Find(x => x.Id == request.Id && !x.Deleted).FirstOrDefaultAsync();
                if(evenT is null) return new(null, 404, "Event não encontrado");
                
                evenT.Deleted = true;
                evenT.DeletedAt = DateTime.UtcNow;
                evenT.DeletedBy = request.DeletedBy;

                await context.Events.ReplaceOneAsync(x => x.Id == request.Id, evenT);

                return new(evenT, 204, "Evento excluída com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
    }
}