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
    public class CustomerRepository(AppDbContext context) : ICustomerRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Customer> pagination)
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

                List<BsonDocument> results = await context.Customers.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<List<dynamic>>> GetMovementAsync(PaginationUtil<Customer> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$addFields", new BsonDocument
                    {
                        {"id", new BsonDocument("$toString", "$_id")},
                    }),
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    new("$skip", pagination.Skip),
                    new("$limit", pagination.Limit),

                    MongoUtil.Lookup("sales_orders", ["$_id"], ["$customerId"], "_salesOrder", [["deleted", false]], 1),
                    MongoUtil.Lookup("accounts_receivable", ["$_id"], ["$customerId"], "_accountsReceivable", [["deleted", false]], 1),
                    MongoUtil.Lookup("service_orders", ["$_id"], ["$customerId"], "_serviceOrders", [["deleted", false]], 1),
                    MongoUtil.Lookup("situations", ["$plan", "$company", "$store"], ["$plan", "$company", "$store"], "_situations", [["deleted", false]]),
                    
                    new("$project", new BsonDocument
                    {
                        {"_id", 0}, 
                        {"tradeName", 1},
                        {"salesOrder", new BsonDocument("$map", new BsonDocument 
                            {
                                {"input", "$_salesOrder"},
                                {"as", "s"},
                                {"in", new BsonDocument 
                                    {
                                        {"id", new BsonDocument("$toString", "$$s._id")},
                                        {"code", "$$s.code"},
                                        {"total", "$$s.total"},
                                        {"status", "$$s.status"},
                                        {"quantity", "$$s.quantity"},
                                        {"createdAt", "$$s.createdAt"},
                                    }
                                }
                            })
                        },
                        {"accountsReceivable", new BsonDocument("$map", new BsonDocument 
                            {
                                {"input", "$_accountsReceivable"},
                                {"as", "s"},
                                {"in", new BsonDocument 
                                    {
                                        {"id", new BsonDocument("$toString", "$$s._id")},
                                        {"code", "$$s.code"},
                                        {"amount", "$$s.amount"},
                                        {"amountPaid", "$$s.amountPaid"},
                                        {"dueDate", "$$s.dueDate"},
                                        {"issueDate", "$$s.issueDate"},
                                        {"status", "$$s.status"},
                                        {"createdAt", "$$s.createdAt"},
                                    }
                                }
                            })
                        },
                        {"serviceOrders", new BsonDocument("$map", new BsonDocument 
                            {
                                {"input", "$_serviceOrders"},
                                {"as", "s"},
                                {"in", new BsonDocument 
                                    {
                                        {"id", new BsonDocument("$toString", "$$s._id")},
                                        {"code", "$$s.code"},
                                        {"totalAmount", "$$s.totalAmount"},
                                        {"status", "$$s.status"},
                                        {"device", "$$s.device"},
                                        {"createdAt", "$$s.createdAt"}
                                    }
                                }
                            })
                        },
                        {"situations", new BsonDocument("$map", new BsonDocument 
                            {
                                {"input", "$_situations"},
                                {"as", "s"},
                                {"in", new BsonDocument 
                                    {
                                        {"id", new BsonDocument("$toString", "$$s._id")},
                                        {"code", "$$s.code"},
                                        {"name", "$$s.name"},
                                        {"createdAt", "$$s.createdAt"}
                                    }
                                }
                            })
                        },
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.Customers.Aggregate<BsonDocument>(pipeline).ToListAsync();
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
                    new("$addFields", new BsonDocument
                    {
                        {"id", new BsonDocument("$toString", "$_id")},
                    }),

                    MongoUtil.Lookup("addresses", ["$id"], ["$parentId"], "_address", [["deleted", false]], 1),

                    new("$addFields", new BsonDocument
                    {
                        {"addressId", MongoUtil.First("_address._id")},
                        {"street",  MongoUtil.First("_address.street")},
                        {"number", MongoUtil.First("_address.number") },
                        {"complement", MongoUtil.First("_address.complement") },
                        {"neighborhood", MongoUtil.First("_address.neighborhood") },
                        {"city", MongoUtil.First("_address.city") },
                        {"state", MongoUtil.First("_address.state") },
                        {"zipCode", MongoUtil.First("_address.zipCode") },
                        {"parent", MongoUtil.First("_address.parent") },
                        {"parentId", MongoUtil.First("_address.parentId") },
                    }),
                    
                    new("$addFields", new BsonDocument
                    {
                        {"address", new BsonDocument
                            {
                                {"id", MongoUtil.ToString("$addressId")},
                                {"street",  MongoUtil.ValidateNull("street", "")},
                                {"number", MongoUtil.ValidateNull("number", "") },
                                {"complement", MongoUtil.ValidateNull("complement", "") },
                                {"neighborhood", MongoUtil.ValidateNull("neighborhood", "") },
                                {"city", MongoUtil.ValidateNull("city", "") },
                                {"state", MongoUtil.ValidateNull("state", "") },
                                {"zipCode", MongoUtil.ValidateNull("zipCode", "") },
                                {"parent", MongoUtil.ValidateNull("parent", "") },
                                {"parentId", MongoUtil.ValidateNull("parentId", "") },
                            }
                        }
                    }),

                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"_address", 0},
                    }),
                ];

                BsonDocument? response = await context.Customers.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Clientes não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        
        public async Task<ResponseApi<Customer?>> GetByIdAsync(string id)
        {
            try
            {
                Customer? customer = await context.Customers.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(customer);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ResponseApi<Customer?>> GetByEmailAsync(string email, string id)
        {
            try
            {
                Customer? customer = new();
                if(!string.IsNullOrEmpty(id))
                {
                    customer = await context.Customers.Find(x => x.Email == email && x.Id != id && !x.Deleted).FirstOrDefaultAsync();
                }
                else
                {
                    customer = await context.Customers.Find(x => x.Email == email && !x.Deleted).FirstOrDefaultAsync();
                };
                return new(customer);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        
        public async Task<ResponseApi<Customer?>> GetByDocumentAsync(string document, string id)
        {
            try
            {
                Customer? customer = new();
                if(!string.IsNullOrEmpty(id))
                {
                    customer = await context.Customers.Find(x => x.Document == document && x.Id != id && !x.Deleted).FirstOrDefaultAsync();
                }
                else
                {
                    customer = await context.Customers.Find(x => x.Document == document && !x.Deleted).FirstOrDefaultAsync();
                };
                return new(customer);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        
        public async Task<int> GetCountDocumentsAsync(PaginationUtil<Customer> pagination)
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

            List<BsonDocument> results = await context.Customers.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<Customer?>> CreateAsync(Customer customer)
        {
            try
            {
                await context.Customers.InsertOneAsync(customer);

                return new(customer, 201, "Clientes criada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<Customer?>> UpdateAsync(Customer customer)
        {
            try
            {
                await context.Customers.ReplaceOneAsync(x => x.Id == customer.Id, customer);

                return new(customer, 201, "Clientes atualizada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<Customer>> DeleteAsync(string id)
        {
            try
            {
                Customer? customer = await context.Customers.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                if(customer is null) return new(null, 404, "Clientes não encontrado");
                customer.Deleted = true;
                customer.DeletedAt = DateTime.UtcNow;

                await context.Customers.ReplaceOneAsync(x => x.Id == id, customer);

                return new(customer, 204, "Clientes excluída com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
    }
}