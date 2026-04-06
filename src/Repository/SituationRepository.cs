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
    public class SituationRepository(AppDbContext context) : ISituationRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Situation> pagination)
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

            List<BsonDocument> results = await context.Situations.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Situações");
        }
    }
    public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Situation> pagination)
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
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.Situations.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Situações");
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

            BsonDocument? response = await context.Situations.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Situações não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Situações");
        }
    }
    
    public async Task<ResponseApi<Situation?>> GetByIdAsync(string id)
    {
        try
        {
            Situation? situation = await context.Situations.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(situation);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Situações");
        }
    }
    public async Task<ResponseApi<List<Situation>>> GetByAppearsOnPanelAsync()
    {
        try
        {
            List<Situation> situations = await context.Situations.Find(x => x.AppearsOnPanel && !x.Deleted).ToListAsync();
            return new(situations);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Situações");
        }
    }
    public async Task<ResponseApi<Situation?>> GetByMomentAsync(string moment)
    {
        try
        {
            Situation? situation = new();
            if(moment == "start") situation = await context.Situations.Find(x => x.Start && !x.Deleted).FirstOrDefaultAsync();
            if(moment == "quite") situation = await context.Situations.Find(x => x.Quite && !x.Deleted).FirstOrDefaultAsync();
            if(moment == "end") situation = await context.Situations.Find(x => x.End && !x.Deleted).FirstOrDefaultAsync();
            if(moment == "appearsOnPanel") situation = await context.Situations.Find(x => x.AppearsOnPanel && !x.Deleted).FirstOrDefaultAsync();
            return new(situation);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Situações");
        }
    }
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<Situation> pagination)
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

        List<BsonDocument> results = await context.Situations.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Situation?>> CreateAsync(Situation situation)
    {
        try
        {
            await context.Situations.InsertOneAsync(situation);

            return new(situation, 201, "Situações criada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Situações");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Situation?>> UpdateAsync(Situation situation)
    {
        try
        {
            await context.Situations.ReplaceOneAsync(x => x.Id == situation.Id, situation);

            return new(situation, 201, "Situações atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Situações");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Situation>> DeleteAsync(string id)
    {
        try
        {
            Situation? situation = await context.Situations.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(situation is null) return new(null, 404, "Situações não encontrado");
            situation.Deleted = true;
            situation.DeletedAt = DateTime.UtcNow;

            await context.Situations.ReplaceOneAsync(x => x.Id == id, situation);

            return new(situation, 204, "Situações excluída com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Situações");
        }
    }
    #endregion
}
}