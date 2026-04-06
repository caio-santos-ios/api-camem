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
    public class ServiceOrderItemRepository(AppDbContext context) : IServiceOrderItemRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<ServiceOrderItem> pagination)
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
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.ServiceOrderItems.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Item da O.S.");
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

            BsonDocument? response = await context.ServiceOrderItems.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Item da O.S. não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Item da O.S.");
        }
    }
    
    public async Task<ResponseApi<ServiceOrderItem?>> GetByIdAsync(string id)
    {
        try
        {
            ServiceOrderItem? items = await context.ServiceOrderItems.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(items);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Item da O.S.");
        }
    }
    public async Task<ResponseApi<List<ServiceOrderItem>>> GetByServiceOrderIdAsync(string serviceOrderId)
    {
        try
        {
            List<ServiceOrderItem> items = await context.ServiceOrderItems.Find(x => x.ServiceOrderId == serviceOrderId && !x.Deleted).ToListAsync();
            return new(items);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Item da O.S.");
        }
    }
    
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<ServiceOrderItem> pagination)
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

        List<BsonDocument> results = await context.ServiceOrderItems.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<ServiceOrderItem?>> CreateAsync(ServiceOrderItem items)
    {
        try
        {
            await context.ServiceOrderItems.InsertOneAsync(items);

            return new(items, 201, "Item da O.S. criada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Item da O.S.");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<ServiceOrderItem?>> UpdateAsync(ServiceOrderItem items)
    {
        try
        {
            await context.ServiceOrderItems.ReplaceOneAsync(x => x.Id == items.Id, items);

            return new(items, 201, "Item da O.S. atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Item da O.S.");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<ServiceOrderItem>> DeleteAsync(string id)
    {
        try
        {
            ServiceOrderItem? items = await context.ServiceOrderItems.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(items is null) return new(null, 404, "Item da O.S. não encontrado");
            items.Deleted = true;
            items.DeletedAt = DateTime.UtcNow;

            await context.ServiceOrderItems.ReplaceOneAsync(x => x.Id == id, items);

            return new(items, 204, "Item da O.S. excluída com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Item da O.S.");
        }
    }
    #endregion
}
}