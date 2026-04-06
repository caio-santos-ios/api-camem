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
    public class ServiceOrderRepository(AppDbContext context) : IServiceOrderRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<ServiceOrder> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$sort", pagination.PipelineSort),
                new("$skip", pagination.Skip),
                new("$limit", pagination.Limit),

                MongoUtil.Lookup("customers", ["$customerId"], ["$_id"], "_customer", [["deleted", false]], 1),
                MongoUtil.Lookup("users", ["$createdBy"], ["$_id"], "_user", [["deleted", false]], 1),
                
                new("$addFields", new BsonDocument {
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"customerName", MongoUtil.First("_customer.tradeName")},
                    {"userName", MongoUtil.First("_user.name")},
                }),

                new("$match", pagination.PipelineFilter),
                

                new("$project", new BsonDocument
                {
                    {"_id", 0},
                    {"_customer", 0},
                    {"_user", 0}
                }),
                
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.ServiceOrders.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Ordens de Serviço");
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

                MongoUtil.Lookup("customers", ["$customerId"], ["$_id"], "_customer", [["deleted", false]], 1),
                MongoUtil.Lookup("situations", ["$status"], ["$_id"], "_situation", [["deleted", false]], 1),
                MongoUtil.Lookup("service_order_items", ["$_id"], ["$serviceOrderId"], "_items", [["deleted", false]], 1),

                new("$addFields", new BsonDocument {
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"customerName", MongoUtil.First("_customer.tradeName")},
                    {"customerEmail", MongoUtil.First("_customer.email")},
                    {"customerPhone", MongoUtil.First("_customer.phone")},
                    {"statusName", MongoUtil.First("_situation.name")},
                    {"situationStyle", MongoUtil.First("_situation.style")},
                    {"items", new BsonDocument("$map", new BsonDocument 
                        {
                            {"input", "$_items"},
                            {"as", "i"},
                            {"in", new BsonDocument 
                                {
                                    {"id", new BsonDocument("$toString", "$$i._id")},
                                }
                            }
                        })
                    }
                }),

                new("$project", new BsonDocument
                {
                    {"_id", 0},
                    {"_customer", 0},
                    {"_situation", 0},
                    {"_items", 0},
                }),
            ];

            BsonDocument? response = await context.ServiceOrders.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Ordem de Serviço não encontrada") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Ordem de Serviço");
        }
    }

    public async Task<ResponseApi<ServiceOrder?>> GetByIdAsync(string id)
    {
        try
        {
            ServiceOrder? serviceOrder = await context.ServiceOrders.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(serviceOrder);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Ordem de Serviço");
        }
    }

    public async Task<int> GetCountDocumentsAsync(PaginationUtil<ServiceOrder> pagination)
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

        List<BsonDocument> results = await context.ServiceOrders.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }

    public async Task<ResponseApi<dynamic?>> CheckWarrantyAsync(string? customerId, string? serialImei)
    {
        try
        {
            var filterBuilder = Builders<ServiceOrder>.Filter;
            var filters = new List<FilterDefinition<ServiceOrder>>
            {
                filterBuilder.Eq(x => x.Deleted, false),
                // filterBuilder.Eq(x => x.IsClosed, true),
                // filterBuilder.Gt(x => x.WarrantyUntil, DateTime.UtcNow),
            };

            if (!string.IsNullOrEmpty(serialImei))
            {
                // filters.Add(filterBuilder.Eq(x => x.Device.SerialImei, serialImei));
            }
            else if (!string.IsNullOrEmpty(customerId))
            {
                filters.Add(filterBuilder.Eq(x => x.CustomerId, customerId));
            }
            else
            {
                return new(null);
            }

            ServiceOrder? found = await context.ServiceOrders.Find(filterBuilder.And(filters)).FirstOrDefaultAsync();
            if (found is null) return new(null);

            dynamic result = new System.Dynamic.ExpandoObject();
            var dict = (IDictionary<string, object>)result;
            dict["id"] = found.Id;
            dict["status"] = found.Status;
            // dict["warrantyUntil"] = found.WarrantyUntil?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? "";
            dict["matchType"] = !string.IsNullOrEmpty(serialImei) ? "serial" : "customer";

            return new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao verificar garantia");
        }
    }
    #endregion

    #region CREATE
    public async Task<ResponseApi<ServiceOrder?>> CreateAsync(ServiceOrder serviceOrder)
    {
        try
        {
            await context.ServiceOrders.InsertOneAsync(serviceOrder);
            return new(serviceOrder, 201, "Ordem de Serviço criada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Ordem de Serviço");
        }
    }
    #endregion

    #region UPDATE
    public async Task<ResponseApi<ServiceOrder?>> UpdateAsync(ServiceOrder serviceOrder)
    {
        try
        {
            await context.ServiceOrders.ReplaceOneAsync(x => x.Id == serviceOrder.Id, serviceOrder);
            return new(serviceOrder, 200, "Ordem de Serviço atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Ordem de Serviço");
        }
    }
    #endregion

    #region DELETE
    public async Task<ResponseApi<ServiceOrder>> DeleteAsync(string id)
    {
        try
        {
            ServiceOrder? serviceOrder = await context.ServiceOrders.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if (serviceOrder is null) return new(null, 404, "Ordem de Serviço não encontrada");
            serviceOrder.Deleted = true;
            serviceOrder.DeletedAt = DateTime.UtcNow;

            await context.ServiceOrders.ReplaceOneAsync(x => x.Id == id, serviceOrder);
            return new(serviceOrder, 204, "Ordem de Serviço excluída com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Ordem de Serviço");
        }
    }
    #endregion
}
}