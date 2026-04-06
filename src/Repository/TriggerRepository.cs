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
    public class TriggerRepository(AppDbContext context) : ITriggerRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Trigger> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    new("$skip", pagination.Skip),
                    new("$limit", pagination.Limit),
                    new("$addFields", new BsonDocument
                    {
                        { "id", new BsonDocument("$toString", "$_id") },
                    }),
                    new("$project", new BsonDocument { { "_id", 0 } }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.Triggers.Aggregate<BsonDocument>(pipeline).ToListAsync();
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
                BsonDocument[] pipeline =
                [
                    new("$match", new BsonDocument { { "_id", new ObjectId(id) }, { "deleted", false } }),
                    new("$addFields", new BsonDocument { { "id", new BsonDocument("$toString", "$_id") } }),
                    new("$project", new BsonDocument { { "_id", 0 } }),
                ];

                BsonDocument? response = await context.Triggers.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Trigger não encontrada.") : new(result);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");   
            }
        }

        public async Task<ResponseApi<Trigger?>> GetByIdAsync(string id)
        {
            try
            {
                Trigger? trigger = await context.Triggers.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(trigger);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");   
            }
        }

        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Trigger> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$sort", pagination.PipelineSort),
                    new("$addFields", new BsonDocument { { "id", new BsonDocument("$toString", "$_id") } }),
                    new("$match", pagination.PipelineFilter),
                    new("$project", new BsonDocument
                    {
                        { "_id", 0 },
                        { "id", 1 },
                        { "code", 1 }
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.Triggers.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");   
            }
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<Trigger> pagination)
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                new("$addFields", new BsonDocument { { "id", new BsonDocument("$toString", "$_id") } }),
                new("$project", new BsonDocument { { "_id", 0 } }),
            };

            List<BsonDocument> results = await context.Triggers.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Count;
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<Trigger?>> CreateAsync(Trigger trigger)
        {
            try
            {
                await context.Triggers.InsertOneAsync(trigger);
                return new(trigger, 201, "Trigger criada com sucesso.");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");   
            }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<Trigger?>> UpdateAsync(Trigger trigger)
        {
            try
            {
                await context.Triggers.ReplaceOneAsync(x => x.Id == trigger.Id, trigger);
                return new(trigger, 200, "Trigger atualizada com sucesso.");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");   
            }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<Trigger>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                Trigger? trigger = await context.Triggers.Find(x => x.Id == request.Id && !x.Deleted).FirstOrDefaultAsync();
                if (trigger is null) return new(null, 404, "Trigger não encontrada.");

                trigger.Deleted = true;
                trigger.DeletedAt = DateTime.UtcNow;
                trigger.DeletedBy = request.DeletedBy;

                await context.Triggers.ReplaceOneAsync(x => x.Id == request.Id, trigger);
                return new(trigger, 204, "Trigger excluída com sucesso.");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");   
            }
        }
        #endregion
    }
}