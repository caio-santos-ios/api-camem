using api_camem.src.Configuration;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.Utils;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;


namespace api_camem.src.Repository
{
    public class CustomCertificateRepository(AppDbContext context) : ICustomCertificateRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<CustomCertificate> pagination)
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
                        {"id", new BsonDocument("$toString", "$_id")},
                    }),
                    
                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"id", 1},
                        {"name", 1},
                        {"createdAt", 1},
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.CustomCertificates.Aggregate<BsonDocument>(pipeline).ToListAsync();
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

                BsonDocument? response = await context.CustomCertificates.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Certificado não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<dynamic?>> GetValidateKeyAsync(string keyCustomCertificate)
        {
            try
            {
                BsonDocument[] pipeline = [
                    new("$match", new BsonDocument{
                        {"keyCustomCertificate", keyCustomCertificate},
                        {"deleted", false}
                    }),
                    
                    MongoUtil.Lookup("events", ["$eventId"], ["$_id"], "_event", [["deleted", false]], 1),
                    MongoUtil.Lookup("users", ["$userId"], ["$_id"], "_user", [["deleted", false]], 1),
                    MongoUtil.Lookup("event_participants", ["$userId"], ["$userId"], "_event_participant", [["deleted", false]], 1),

                    new("$addFields", new BsonDocument
                    {
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"nameEvent", MongoUtil.First("_event.title")},
                        {"startDate", MongoUtil.First("_event.startDate")},
                        {"endDate", MongoUtil.First("_event.endDate")},
                        {"name", MongoUtil.First("_user.name")},
                    }),

                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"id", 1},
                        {"startDate", 1},
                        {"endDate", 1},
                        {"hours", 1},
                        {"nameEvent", 1},
                        {"name", 1},
                        {"keyCustomCertificate", 1}
                    }),
                ];

                BsonDocument? response = await context.CustomCertificates.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Certificado não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        
        public async Task<ResponseApi<CustomCertificate?>> GetByIdAsync(string id)
        {
            try
            {
                CustomCertificate? paymentMethod = await context.CustomCertificates.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(paymentMethod);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        
        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<CustomCertificate> pagination)
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

                List<BsonDocument> results = await context.CustomCertificates.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<CustomCertificate> pagination)
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

            List<BsonDocument> results = await context.CustomCertificates.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<CustomCertificate?>> CreateAsync(CustomCertificate paymentMethod)
        {
            try
            {
                await context.CustomCertificates.InsertOneAsync(paymentMethod);

                return new(paymentMethod, 201, "Certificado criada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");  
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<CustomCertificate?>> UpdateAsync(CustomCertificate paymentMethod)
        {
            try
            {
                await context.CustomCertificates.ReplaceOneAsync(x => x.Id == paymentMethod.Id, paymentMethod);

                return new(paymentMethod, 200, "Certificado atualizada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<CustomCertificate>> DeleteAsync(string id)
        {
            try
            {
                CustomCertificate? paymentMethod = await context.CustomCertificates.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                if(paymentMethod is null) return new(null, 404, "Certificado não encontrado");
                paymentMethod.Deleted = true;
                paymentMethod.DeletedAt = DateTime.UtcNow;

                await context.CustomCertificates.ReplaceOneAsync(x => x.Id == id, paymentMethod);

                return new(paymentMethod, 204, "Certificado excluída com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
    }
}