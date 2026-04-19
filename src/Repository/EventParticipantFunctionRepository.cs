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
    public class EventParticipantFunctionRepository(AppDbContext context) : IEventParticipantFunctionRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<EventParticipantFunction> pagination)
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

                    new("$addFields", new BsonDocument
                    {
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"userCpf", MongoUtil.First("_user.cpf")},
                        {"userRa", MongoUtil.First("_user.ra")},
                    }),

                    new("$project", new BsonDocument
                    {
                        {"_id", 0}, 
                        {"id", 1},
                        {"name", 1},
                        {"hours", 1},
                        {"createdAt", 1},
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.EventParticipantFunctions.Aggregate<BsonDocument>(pipeline).ToListAsync();
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

                BsonDocument? response = await context.EventParticipantFunctions.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Função não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }   
        public async Task<ResponseApi<EventParticipantFunction?>> GetByIdAsync(string id)
        {
            try
            {
                EventParticipantFunction? eventParticipant = await context.EventParticipantFunctions.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(eventParticipant);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        } 
        public async Task<ResponseApi<List<EventParticipantFunction>>> GetByEventParticipantIdAsync(string eventParticipantId)
        {
            try
            {
                List<EventParticipantFunction> eventParticipantFunctions = await context.EventParticipantFunctions.Find(x => x.EventParticipantId == eventParticipantId && !x.Deleted).ToListAsync();
                return new(eventParticipantFunctions);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        } 
        public async Task<ResponseApi<List<EventParticipantFunction>>> GetAllByEventIdAsync(string eventId)
        {
            try
            {
                List<EventParticipantFunction> eventParticipants = await context.EventParticipantFunctions.Find(x => x.EventId == eventId && !x.Deleted).ToListAsync();
                return new(eventParticipants);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        } 
        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<EventParticipantFunction> pagination)
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
                        {"name", 1},
                        {"hours", 1}, 
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.EventParticipantFunctions.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<int> GetCountDocumentsAsync(PaginationUtil<EventParticipantFunction> pagination)
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

            List<BsonDocument> results = await context.EventParticipantFunctions.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<EventParticipantFunction?>> CreateAsync(EventParticipantFunction eventParticipant)
        {
            try
            {
                await context.EventParticipantFunctions.InsertOneAsync(eventParticipant);

                return new(eventParticipant, 201, "Função criado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<EventParticipantFunction?>> UpdateAsync(EventParticipantFunction eventParticipant)
        {
            try
            {
                await context.EventParticipantFunctions.ReplaceOneAsync(x => x.Id == eventParticipant.Id, eventParticipant);

                return new(eventParticipant, 200, "Função atualizada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<EventParticipantFunction>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                EventParticipantFunction? eventParticipant = await context.EventParticipantFunctions.Find(x => x.Id == request.Id && !x.Deleted).FirstOrDefaultAsync();
                if(eventParticipant is null) return new(null, 404, "Função não encontrado");
                
                eventParticipant.Deleted = true;
                eventParticipant.DeletedAt = DateTime.UtcNow;
                eventParticipant.DeletedBy = request.DeletedBy;

                await context.EventParticipantFunctions.ReplaceOneAsync(x => x.Id == request.Id, eventParticipant);

                return new(eventParticipant, 204, "Função excluída com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
    }
}