using api_camem.src.Handlers;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;
using AutoMapper;

namespace api_camem.src.Services
{
    public class ProfileUserService
    (
        IProfileUserRepository repository, 
        CountHandler countHandler, 
        IMapper _mapper
    ) : IProfileUserService
    {
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<ProfileUser> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> profileUsers = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                return new(profileUsers.Data, count, pagination.PageNumber, pagination.PageSize);
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
                ResponseApi<dynamic?> ProfileUser = await repository.GetByIdAggregateAsync(id);
                if(ProfileUser.Data is null) return new(null, 404, "Perfil de Usuário não encontrada");
                return new(ProfileUser.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<ProfileUser> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> profileUsers = await repository.GetSelectAsync(pagination);
                return new(profileUsers.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        } 
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<ProfileUser?>> CreateAsync(CreateProfileUserDTO request)
        {
            try
            {
                ProfileUser profileUser = _mapper.Map<ProfileUser>(request);

                profileUser.Code = await countHandler.NextCountAsync("profile_user");
                ResponseApi<ProfileUser?> response = await repository.CreateAsync(profileUser);

                if(response.Data is null) return new(null, 400, "Falha ao criar Perfil de Usuário.");
                return new(response.Data, 201, "Perfil de Usuário criada com sucesso.");
            }
            catch
            { 
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }

        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<ProfileUser?>> UpdateAsync(UpdateProfileUserDTO request)
        {
            try
            {
                ResponseApi<ProfileUser?> profileUserResponse = await repository.GetByIdAsync(request.Id);
                if(profileUserResponse.Data is null) return new(null, 404, "Falha ao atualizar");
                
                ProfileUser profileUser = _mapper.Map<ProfileUser>(request);
                profileUser.UpdatedAt = DateTime.UtcNow;
                profileUser.CreatedAt = profileUserResponse.Data.CreatedAt;
                profileUser.Code = profileUserResponse.Data.Code;

                ResponseApi<ProfileUser?> response = await repository.UpdateAsync(profileUser);
                if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
                return new(response.Data, 200, "Atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<ProfileUser>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                ResponseApi<ProfileUser> profileUser = await repository.DeleteAsync(request);
                if(!profileUser.IsSuccess) return new(null, 400, profileUser.Message);
                return new(null, 204, "Excluída com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion 
    }
}