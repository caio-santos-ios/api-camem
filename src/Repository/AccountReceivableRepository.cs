using api_camem.src.Configuration;
using api_camem.src.Interfaces;
using api_camem.src.Models.Base;
using api_camem.src.Shared.Utils;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace api_camem.src.Repository
{
    public class AccountReceivableRepository(AppDbContext context) : IAccountReceivableRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Models.AccountReceivable> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    new("$skip", pagination.Skip),
                    new("$limit", pagination.Limit),

                    MongoUtil.Lookup("payment_methods", ["$paymentMethodId"], ["$_id"], "_paymentMethod", [["deleted", false]], 1),
                    MongoUtil.Lookup("customers", ["$customerId"], ["$_id"], "_customer", [["deleted", false]], 1),
                    
                    new("$addFields", new BsonDocument
                    {
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"paymentMethodName", MongoUtil.First("_paymentMethod.name")},
                        {"customerName", MongoUtil.First("_customer.corporateName")},
                    }),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"_paymentMethod", 0},
                        {"_customer", 0},
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.AccountsReceivable.Aggregate<BsonDocument>(pipeline).ToListAsync();
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

                BsonDocument? response = await context.AccountsReceivable.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Conta a receber não encontrada") : new(result);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ResponseApi<Models.AccountReceivable?>> GetByIdAsync(string id)
        {
            try
            {
                Models.AccountReceivable? accountReceivable = await context.AccountsReceivable
                    .Find(x => x.Id == id && !x.Deleted)
                    .FirstOrDefaultAsync();
                return new(accountReceivable);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<Models.AccountReceivable> pagination)
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

            List<BsonDocument> results = await context.AccountsReceivable.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<Models.AccountReceivable?>> CreateAsync(Models.AccountReceivable accountReceivable)
        {
            try
            {
                await context.AccountsReceivable.InsertOneAsync(accountReceivable);
                return new(accountReceivable, 201, "Conta a receber criada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<Models.AccountReceivable?>> UpdateAsync(Models.AccountReceivable accountReceivable)
        {
            try
            {
                await context.AccountsReceivable.ReplaceOneAsync(x => x.Id == accountReceivable.Id, accountReceivable);
                return new(accountReceivable, 200, "Conta a receber atualizada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ResponseApi<Models.AccountReceivable?>> PayAsync(Models.AccountReceivable accountReceivable)
        {
            try
            {
                await context.AccountsReceivable.ReplaceOneAsync(x => x.Id == accountReceivable.Id, accountReceivable);
                return new(accountReceivable, 200, "Título baixado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<Models.AccountReceivable>> DeleteAsync(string id)
        {
            try
            {
                Models.AccountReceivable? accountReceivable = await context.AccountsReceivable
                    .Find(x => x.Id == id && !x.Deleted)
                    .FirstOrDefaultAsync();

                if (accountReceivable is null) return new(null, 404, "Conta a receber não encontrada");

                accountReceivable.Deleted = true;
                accountReceivable.DeletedAt = DateTime.UtcNow;

                await context.AccountsReceivable.ReplaceOneAsync(x => x.Id == id, accountReceivable);
                return new(accountReceivable, 204, "Conta a receber excluída com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
    }
}
