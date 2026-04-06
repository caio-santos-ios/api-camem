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
    public class SupplierRepository(AppDbContext context) : ISupplierRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Supplier> pagination)
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

                List<BsonDocument> results = await context.Suppliers.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<List<dynamic>>> GetAutocompleteAsync(PaginationUtil<Supplier> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    new("$skip", pagination.Skip),
                    new("$limit", pagination.Limit),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0}, 
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"corporateName", 1}
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.Suppliers.Aggregate<BsonDocument>(pipeline).ToListAsync();
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

                    new BsonDocument("$lookup", new BsonDocument
                    {
                        { "from", "addresses" },

                        { "let", new BsonDocument("id", new BsonDocument("$toString", "$_id")) },

                        { "pipeline", new BsonArray
                            {
                                new BsonDocument("$match", new BsonDocument
                                {
                                    { "$expr", new BsonDocument("$and", new BsonArray
                                        {
                                            new BsonDocument("$eq", new BsonArray
                                            {
                                                "$parentId",
                                                "$$id"
                                            }),

                                            new BsonDocument("$eq", new BsonArray
                                            {
                                                "$parent",
                                                "supplier"
                                            })
                                        })
                                    }
                                })
                            }
                        },

                        { "as", "_address" }
                    }),
                    
                    new("$addFields", new BsonDocument {
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"address", new BsonDocument
                            {
                                {"id", new BsonDocument("$toString", new BsonDocument("$first", "$_address._id"))},
                                {"street", new BsonDocument("$first", "$_address.street")},
                                {"number", new BsonDocument("$first", "$_address.number")},
                                {"complement", new BsonDocument("$first", "$_address.complement")},
                                {"neighborhood", new BsonDocument("$first", "$_address.neighborhood")},
                                {"city", new BsonDocument("$first", "$_address.city")},
                                {"state", new BsonDocument("$first", "$_address.state")},
                                {"zipCode", new BsonDocument("$first", "$_address.zipCode")},
                                {"parent", new BsonDocument("$first", "$_address.parent")},
                                {"parentId", new BsonDocument("$first", "$_address.parentId")},
                            }
                        }
                    }),

                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                    }),
                ];

                BsonDocument? response = await context.Suppliers.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Fornecedor não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }        
        public async Task<ResponseApi<Supplier?>> GetByIdAsync(string id)
        {
            try
            {
                Supplier? billing = await context.Suppliers.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(billing);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        
        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Supplier> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    new("$skip", pagination.Skip),
                    new("$limit", pagination.Limit),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0}, 
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"tradeName", 1},
                        {"corporateName", 1},
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.Suppliers.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<Supplier?>> GetByEmailAsync(string email, string id)
        {
            try
            {
                Supplier? supplier = new();
                if(!string.IsNullOrEmpty(id))
                {
                    supplier = await context.Suppliers.Find(x => x.Email == email && x.Id != id && !x.Deleted).FirstOrDefaultAsync();
                }
                else
                {
                    supplier = await context.Suppliers.Find(x => x.Email == email && !x.Deleted).FirstOrDefaultAsync();
                };
                return new(supplier);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }   
    
        public async Task<ResponseApi<Supplier?>> GetByDocumentAsync(string document, string id)
        {
            try
            {
                Supplier? supplier = new();
                if(!string.IsNullOrEmpty(id))
                {
                    supplier = await context.Suppliers.Find(x => x.Document == document && x.Id != id && !x.Deleted).FirstOrDefaultAsync();
                }
                else
                {
                    supplier = await context.Suppliers.Find(x => x.Document == document && !x.Deleted).FirstOrDefaultAsync();
                };
                return new(supplier);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
    
        public async Task<int> GetCountDocumentsAsync(PaginationUtil<Supplier> pagination)
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

            List<BsonDocument> results = await context.Suppliers.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<Supplier?>> CreateAsync(Supplier billing)
        {
            try
            {
                await context.Suppliers.InsertOneAsync(billing);

                return new(billing, 201, "Fornecedor criado com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao criar Fornecedor");  
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<Supplier?>> UpdateAsync(Supplier billing)
        {
            try
            {
                await context.Suppliers.ReplaceOneAsync(x => x.Id == billing.Id, billing);

                return new(billing, 201, "Fornecedor atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<Supplier>> DeleteAsync(string id)
        {
            try
            {
                Supplier? billing = await context.Suppliers.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                if(billing is null) return new(null, 404, "Fornecedor não encontrado");
                billing.Deleted = true;
                billing.DeletedAt = DateTime.UtcNow;

                await context.Suppliers.ReplaceOneAsync(x => x.Id == id, billing);

                return new(billing, 204, "Fornecedor excluído com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
    }
}