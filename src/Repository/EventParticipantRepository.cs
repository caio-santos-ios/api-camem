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
    public class EventParticipantRepository(AppDbContext context) : IEventParticipantRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<EventParticipant> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    new("$skip", pagination.Skip),
                    new("$limit", pagination.Limit),

                    MongoUtil.Lookup("users", ["$userId"], ["$_id"], "_user", [["deleted", false]], 1),
                    MongoUtil.Lookup("event_participant_functions", ["$functionId"], ["$_id"], "_function", [["deleted", false]], 1),

                    new("$addFields", new BsonDocument
                    {
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"userCpf", MongoUtil.First("_user.cpf")},
                        {"userRa", MongoUtil.First("_user.ra")},
                        {"functionName", MongoUtil.First("_function.name")},
                        {"isPresence", MongoUtil.First("_function.isPresence")},
                        {"notesPresence", MongoUtil.First("_function.notesPresence")},
                    }),

                    new("$project", new BsonDocument
                    {
                        {"_id", 0}, 
                        {"id", 1},
                        {"name", 1},
                        {"userCpf", 1},
                        {"userRa", 1},
                        {"functionName", 1},
                        {"isPresence", 1},
                        {"notesPresence", 1},
                        {"hours", 1},
                        {"functionId", 1},
                        {"createdAt", 1},
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.EventParticipants.Aggregate<BsonDocument>(pipeline).ToListAsync();
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

                    MongoUtil.Lookup("users", ["$userId"], ["$_id"], "_user", [["deleted", false]], 1),

                    new("$addFields", new BsonDocument
                    {
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"userName", MongoUtil.First("_user.name")},
                    }),

                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                    }),
                ];

                BsonDocument? response = await context.EventParticipants.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Participante não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }   
        public async Task<ResponseApi<EventParticipant?>> GetByIdAsync(string id)
        {
            try
            {
                EventParticipant? eventParticipant = await context.EventParticipants.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(eventParticipant);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        } 
        public async Task<ResponseApi<List<EventParticipant>>> GetAllByEventIdAsync(string eventId)
        {
            try
            {
                List<EventParticipant> eventParticipants = await context.EventParticipants.Find(x => x.EventId == eventId && !x.Deleted).ToListAsync();
                return new(eventParticipants);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        } 
        public async Task<ResponseApi<EventParticipant?>> GetByUserIdAsync(string userId, string eventId, string id)
        {
            try
            {
                EventParticipant? eventParticipant = null;

                if(string.IsNullOrEmpty(id))
                {
                    eventParticipant = await context.EventParticipants.Find(x => x.UserId == userId && x.EventId == eventId && !x.Deleted).FirstOrDefaultAsync();
                }
                else
                {
                    eventParticipant = await context.EventParticipants.Find(x => x.UserId == userId && x.EventId == eventId && x.Id != id && !x.Deleted).FirstOrDefaultAsync();
                }
                
                return new(eventParticipant);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        } 
        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<EventParticipant> pagination)
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

                List<BsonDocument> results = await context.EventParticipants.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<int> GetCountDocumentsAsync(PaginationUtil<EventParticipant> pagination)
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

            List<BsonDocument> results = await context.EventParticipants.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<EventParticipant?>> CreateAsync(EventParticipant eventParticipant)
        {
            try
            {
                await context.EventParticipants.InsertOneAsync(eventParticipant);

                return new(eventParticipant, 201, "Participante criado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<EventParticipant?>> UpdateAsync(EventParticipant eventParticipant)
        {
            try
            {
                await context.EventParticipants.ReplaceOneAsync(x => x.Id == eventParticipant.Id, eventParticipant);

                return new(eventParticipant, 200, "Participante atualizada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<EventParticipant>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                EventParticipant? eventParticipant = await context.EventParticipants.Find(x => x.Id == request.Id && !x.Deleted).FirstOrDefaultAsync();
                if(eventParticipant is null) return new(null, 404, "Participante não encontrado");
                
                eventParticipant.Deleted = true;
                eventParticipant.DeletedAt = DateTime.UtcNow;
                eventParticipant.DeletedBy = request.DeletedBy;

                await context.EventParticipants.ReplaceOneAsync(x => x.Id == request.Id, eventParticipant);

                return new(eventParticipant, 204, "Participante excluída com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
    }
}