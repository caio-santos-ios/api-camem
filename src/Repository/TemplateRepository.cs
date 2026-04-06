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
    public class TemplateRepository(AppDbContext context) : ITemplateRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Template> pagination)
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

                List<BsonDocument> results = await context.Templates.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Template");
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

                    new("$addFields", new BsonDocument
                    {
                        {"id", new BsonDocument("$toString", "$_id")},
                    }),

                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                    }),
                ];

                BsonDocument? response = await context.Templates.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Template não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Template");
            }
        }   
        public async Task<ResponseApi<Template?>> GetByIdAsync(string id)
        {
            try
            {
                Template? template = await context.Templates.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(template);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Template");
            }
        } 
        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Template> pagination)
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

                List<BsonDocument> results = await context.Templates.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Empresas");
            }
        }
        public async Task<int> GetCountDocumentsAsync(PaginationUtil<Template> pagination)
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

            List<BsonDocument> results = await context.Templates.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<Template?>> CreateAsync(Template template)
        {
            try
            {
                await context.Templates.InsertOneAsync(template);

                return new(template, 201, "Template criada com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao criar Template");  
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<Template?>> UpdateAsync(Template template)
        {
            try
            {
                await context.Templates.ReplaceOneAsync(x => x.Id == template.Id, template);

                return new(template, 201, "Template atualizada com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao atualizar Template");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<Template>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                Template? template = await context.Templates.Find(x => x.Id == request.Id && !x.Deleted).FirstOrDefaultAsync();
                if(template is null) return new(null, 404, "Template não encontrado");
                
                template.Deleted = true;
                template.DeletedAt = DateTime.UtcNow;
                template.DeletedBy = request.DeletedBy;

                await context.Templates.ReplaceOneAsync(x => x.Id == request.Id, template);

                return new(template, 204, "Template excluída com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao excluír Template");
            }
        }
        #endregion
    }
}