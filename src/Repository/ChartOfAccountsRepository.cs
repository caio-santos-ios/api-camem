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
    public class ChartOfAccountsRepository(AppDbContext context) : IChartOfAccountsRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<ChartOfAccounts> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"name", 1},
                        {"code", 1},
                        {"type", 1},
                        {"groupDRE", 1},
                        {"account", 1},
                        {"level", 1},
                        {"createdAt", 1},
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.ChartOfAccounts.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Plano de Contas");
            }
        }
        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<ChartOfAccounts> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    MongoUtil.Lookup("chart_of_accounts", ["$parentId"], ["$_id"], "_parent", [["deleted", false]], 1),
                    new("$addFields", new BsonDocument
                    {
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"parentName", MongoUtil.First("_parent.name")},
                    }),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"name", 1},
                        {"code", 1},
                        {"parentName", 1},
                        {"level", 1},
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.ChartOfAccounts.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Plano de Contas");
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
                    MongoUtil.Lookup("chart_of_accounts", ["$parentId"], ["$_id"], "_parent", [["deleted", false]], 1),
                    new("$addFields", new BsonDocument {
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"parentName", MongoUtil.First("_parent.name")},
                    }),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"_parent", 0},
                    }),
                ];

                BsonDocument? result = await context.ChartOfAccounts.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? data = result is not null ? BsonSerializer.Deserialize<dynamic>(result) : null;
                return new(data);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Conta");
            }
        }
        public async Task<ResponseApi<ChartOfAccounts?>> GetByIdAsync(string id)
        {
            try
            {
                FilterDefinition<ChartOfAccounts> filter = Builders<ChartOfAccounts>.Filter.And(
                    Builders<ChartOfAccounts>.Filter.Eq("_id", new ObjectId(id)),
                    Builders<ChartOfAccounts>.Filter.Eq("deleted", false)
                );

                ChartOfAccounts? obj = await context.ChartOfAccounts.Find(filter).FirstOrDefaultAsync();
                return new(obj);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Conta");
            }
        }
        public async Task<int> GetCountDocumentsAsync(PaginationUtil<ChartOfAccounts> pagination)
        {
            try
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
                        {"createdAt", 1},
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.ChartOfAccounts.Aggregate<BsonDocument>(pipeline).ToListAsync();
                return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
            }
            catch
            {
                return 0;
            }
        }
        public async Task<ResponseApi<long>> GetNextCodeAsync(string type, string groupDRE)
        {
            try
            {
                FilterDefinition<ChartOfAccounts> filter = Builders<ChartOfAccounts>.Filter.And(
                    Builders<ChartOfAccounts>.Filter.Eq("type", type),
                    Builders<ChartOfAccounts>.Filter.Eq("groupDRE", groupDRE),
                    Builders<ChartOfAccounts>.Filter.Eq("deleted", false)
                );

                long count = await context.ChartOfAccounts.CountDocumentsAsync(filter);

                return new(count + 1);
            }
            catch
            {
                return new(1, 500, "Falha ao buscar próximo código");
            }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<ChartOfAccounts?>> CreateAsync(ChartOfAccounts chartOfAccounts)
        {
            try
            {
                await context.ChartOfAccounts.InsertOneAsync(chartOfAccounts);
                return new(chartOfAccounts);
            }
            catch
            {
                return new(null, 500, "Falha ao criar Conta");
            }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<ChartOfAccounts?>> UpdateAsync(ChartOfAccounts chartOfAccounts)
        {
            try
            {
                FilterDefinition<ChartOfAccounts> filter = Builders<ChartOfAccounts>.Filter.Eq("_id", new ObjectId(chartOfAccounts.Id));
                await context.ChartOfAccounts.ReplaceOneAsync(filter, chartOfAccounts);
                return new(chartOfAccounts);
            }
            catch
            {
                return new(null, 500, "Falha ao atualizar Conta");
            }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<ChartOfAccounts>> DeleteAsync(string id)
        {
            try
            {
                FilterDefinition<ChartOfAccounts> filter = Builders<ChartOfAccounts>.Filter.Eq("_id", new ObjectId(id));
                ChartOfAccounts? chartOfAccounts = await context.ChartOfAccounts.Find(filter).FirstOrDefaultAsync();

                if (chartOfAccounts is null)
                {
                    return new(null!, 404, "Conta não encontrada");
                }

                chartOfAccounts.Deleted = true;
                await context.ChartOfAccounts.ReplaceOneAsync(filter, chartOfAccounts);
                return new(chartOfAccounts);
            }
            catch
            {
                return new(null!, 500, "Falha ao deletar Conta");
            }
        }
        #endregion
    }
}