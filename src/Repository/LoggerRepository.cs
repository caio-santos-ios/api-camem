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
    public class LoggerRepository(AppDbContext context) : ILoggerRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Logger> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    new("$skip", pagination.Skip),
                    new("$limit", pagination.Limit),
                    
                    MongoUtil.Lookup("users", ["$createdBy"], ["$_id"], "_user", [["deleted", false]], 1),

                    new("$addFields", new BsonDocument 
                    {
                        {"userName", MongoUtil.First("_user.name")}
                    }),

                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"method", 1},
                        {"message", 1},
                        {"statusCode", 1},
                        {"audit", 1},
                        {"path", 1},
                        {"createdAt", 1},
                        {"userName", 1},
                        {"time", 1},
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.Loggers.Aggregate<BsonDocument>(pipeline).ToListAsync();
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
                    new("$addFields", new BsonDocument {
                        {"id", new BsonDocument("$toString", "$_id")},
                    }),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                    }),
                ];

                BsonDocument? response = await context.Loggers.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Log não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<Logger?>> GetByIdAsync(string id)
        {
            try
            {
                Logger? logger = await context.Loggers.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(logger);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<int> GetCountDocumentsAsync(PaginationUtil<Logger> pagination)
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

            List<BsonDocument> results = await context.Loggers.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion
        #region CREATE
        public async Task<ResponseApi<Logger?>> CreateAsync(Logger logger)
        {
            try
            {
                await context.Loggers.InsertOneAsync(logger);
                return new(logger, 201, "Log criado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");   
            }
        }
        #endregion
        #region UPDATE
        public async Task<ResponseApi<Logger?>> UpdateAsync(Logger logger)
        {
            try
            {
                await context.Loggers.ReplaceOneAsync(x => x.Id == logger.Id, logger);
                return new(logger, 200, "Log atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        #region DELETE
        public async Task<ResponseApi<Logger>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                Logger? logger = await context.Loggers.Find(x => x.Id == request.Id && !x.Deleted).FirstOrDefaultAsync();
                if(logger is null) return new(null, 404, "Log não encontrado");
                logger.Deleted = true;
                logger.DeletedAt = DateTime.UtcNow;
                logger.DeletedBy = request.DeletedBy;

                await context.Loggers.ReplaceOneAsync(x => x.Id == request.Id, logger);

                return new(logger, 204, "Log excluído com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
    }
}