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
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        #region CREATE
        public async Task<ResponseApi<User?>> CreateAsync(User user)
        {
            try
            {
                await context.Users.InsertOneAsync(user);
                return new(user, 201, "Usuário criado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");   
            }
        }
        #endregion
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<User> pagination)
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
                        {"name", 1},
                        {"email", 1},
                        {"admin", 1},
                        {"blocked", 1},
                        {"photo", 1},
                        {"createdAt", 1},
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.Users.Aggregate<BsonDocument>(pipeline).ToListAsync();
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
                        {"password", 0},
                    }),
                ];

                BsonDocument? response = await context.Users.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Usuário não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<dynamic?>> GetLoggedAsync(string id)
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
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"name", 1},
                        {"email", 1},
                        {"modules", 1},
                        {"admin", 1},
                        {"blocked", 1},
                        {"photo", 1},
                        {"createdAt", 1}
                    }),
                ];

                BsonDocument? response = await context.Users.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Usuário não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<User?>> GetByIdAsync(string id)
        {
            try
            {
                User? user = await context.Users.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(user);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<User?>> GetByUserNameAsync(string userName)
        {
            try
            {
                User? user = await context.Users.Find(x => x.UserName == userName && !x.Deleted).FirstOrDefaultAsync();
                return new(user);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<User?>> GetByEmailAsync(string email)
        {
            try
            {
                User? user = await context.Users.Find(x => x.Email == email && !x.Deleted).FirstOrDefaultAsync();
                return new(user);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<User?>> GetByCodeAccessAsync(string codeAccess)
        {
            try
            {
                User? user = await context.Users.Find(x => x.CodeAccess == codeAccess && !x.ValidatedAccess && !x.Deleted).FirstOrDefaultAsync();
                return new(user);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<bool> GetAccessValitedAsync(string id)
        {
            try
            {
                User? user = await context.Users.Find(x => x.Id == id && !x.Deleted && !x.Blocked && x.ValidatedAccess).FirstOrDefaultAsync();
                if(user is null) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<int> GetCountDocumentsAsync(PaginationUtil<User> pagination)
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
                    {"password", 0},
                    {"role", 0},
                    {"blocked", 0},
                    {"codeAccess", 0},
                    {"validatedAccess", 0}
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.Users.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion
        #region UPDATE
        public async Task<ResponseApi<User?>> UpdateCodeAccessAsync(string userId, string codeAccess)
        {
            try
            {
                User? user = await context.Users.Find(x => x.Id == userId && !x.Deleted && !x.Blocked && !x.ValidatedAccess).FirstOrDefaultAsync();
                if(user is null) return new(null, 404, "Usuário não encontrado");
                user.UpdatedAt = DateTime.UtcNow;
                user.CodeAccess = codeAccess;
                user.ValidatedAccess = false;

                await context.Users.ReplaceOneAsync(x => x.Id == userId, user);

                return new(user, 204, "Código de acesso atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<User?>> UpdateAsync(User user)
        {
            try
            {
                await context.Users.ReplaceOneAsync(x => x.Id == user.Id, user);
                user.Password = "";
                return new(user, 201, "Usuário atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<User?>> ValidatedAccessAsync(string codeAccess)
        {
            try
            {
                User? user = await context.Users.Find(x => x.CodeAccess == codeAccess && !x.Deleted && !x.Blocked && !x.ValidatedAccess).FirstOrDefaultAsync();
                if(user is null) return new(null, 404, "Usuário não encontrado");
                user.UpdatedAt = DateTime.UtcNow;
                user.CodeAccess = "";
                user.ValidatedAccess = true;
                user.CodeAccessExpiration = null;
                
                await context.Users.ReplaceOneAsync(x => x.Id == user.Id, user);

                return new(user, 204, "Código de acesso confirmado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        #region DELETE
        public async Task<ResponseApi<User>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                User? user = await context.Users.Find(x => x.Id == request.Id && !x.Deleted).FirstOrDefaultAsync();
                if(user is null) return new(null, 404, "Usuário não encontrado");
                
                user.Deleted = true;
                user.DeletedAt = DateTime.UtcNow;
                user.DeletedBy = request.DeletedBy;

                await context.Users.ReplaceOneAsync(x => x.Id == user.Id, user);

                return new(user, 204, "Usuário excluído com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
    }
}