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
    public class ProfileUserRepository(AppDbContext context) : IProfileUserRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<ProfileUser> pagination)
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

                List<BsonDocument> results = await context.ProfileUsers.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Perfil de Usuário");
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

                BsonDocument? response = await context.ProfileUsers.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Perfil de Usuário não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Perfil de Usuário");
            }
        }   
        public async Task<ResponseApi<ProfileUser?>> GetByIdAsync(string id)
        {
            try
            {
                ProfileUser? profileUser = await context.ProfileUsers.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(profileUser);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Perfil de Usuário");
            }
        } 
        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<ProfileUser> pagination)
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

                List<BsonDocument> results = await context.ProfileUsers.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Empresas");
            }
        }
        public async Task<int> GetCountDocumentsAsync(PaginationUtil<ProfileUser> pagination)
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

            List<BsonDocument> results = await context.ProfileUsers.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<ProfileUser?>> CreateAsync(ProfileUser profileUser)
        {
            try
            {
                await context.ProfileUsers.InsertOneAsync(profileUser);

                return new(profileUser, 201, "Perfil de Usuário criada com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao criar Perfil de Usuário");  
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<ProfileUser?>> UpdateAsync(ProfileUser profileUser)
        {
            try
            {
                await context.ProfileUsers.ReplaceOneAsync(x => x.Id == profileUser.Id, profileUser);

                return new(profileUser, 200, "Perfil de Usuário atualizada com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao atualizar Perfil de Usuário");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<ProfileUser>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                ProfileUser? profileUser = await context.ProfileUsers.Find(x => x.Id == request.Id && !x.Deleted).FirstOrDefaultAsync();
                if(profileUser is null) return new(null, 404, "Perfil de Usuário não encontrado");
                
                profileUser.Deleted = true;
                profileUser.DeletedAt = DateTime.UtcNow;
                profileUser.DeletedBy = request.DeletedBy;

                await context.ProfileUsers.ReplaceOneAsync(x => x.Id == request.Id, profileUser);

                return new(profileUser, 204, "Perfil de Usuário excluída com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao excluír Perfil de Usuário");
            }
        }
        #endregion
    }
}