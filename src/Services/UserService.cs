using api_camem.src.Handlers;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;
using api_camem.src.Shared.Validators;

namespace api_camem.src.Services
{
    public class UserService(IUserRepository userRepository, IProfileUserRepository profileUserRepository, MailHandler mailHandler, UploadHandler uploadHander) : IUserService
    {
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request, string userId)
        {
            try
            {
                PaginationUtil<User> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> users = await userRepository.GetAllAsync(pagination);
                int count = await userRepository.GetCountDocumentsAsync(pagination);
                return new(users.Data, count, pagination.PageNumber, pagination.PageSize);
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        
        public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
        {
            try
            {
                ResponseApi<dynamic?> user = await userRepository.GetByIdAggregateAsync(id);
                if(user.Data is null) return new(null, 404, "Usuário não encontrado");
                return new(user.Data, 200, "Usuário encontrado");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        
        public async Task<ResponseApi<dynamic?>> GetLoggedAsync(string id)
        {
            try
            {
                ResponseApi<dynamic?> user = await userRepository.GetLoggedAsync(id);
                if(user.Data is null) return new(null, 404, "Usuário não encontrado");
                return new(user.Data, 200, "Usuário encontrado");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<User?>> CreateAsync(CreateUserDTO request)
        {
            try
            {
                ResponseApi<User?> isEmail = await userRepository.GetByEmailAsync(request.Email);
                if(isEmail.Data is not null || !Validator.IsEmail(request.Email)) return new(null, 400, "E-mail inválido.");

                ResponseApi<ProfileUser?> profile = await profileUserRepository.GetByIdAsync(request.ProfileUserId);

                if (profile.Data is null) return new(null, 404, "Perfil de usuário não encontrado.");

                dynamic access = Util.GenerateCodeAccess();

                User user = new()
                {
                    UserName = $"usuário{access.CodeAccess}",
                    Email = request.Email,
                    Name = request.Name,
                    Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    CodeAccess = "",
                    CodeAccessExpiration = null,
                    ValidatedAccess = true,
                    Modules = profile.Data.Modules,
                    Admin = request.Admin,
                    Blocked = request.Blocked,
                    ProfileUserId = request.ProfileUserId,
                };

                ResponseApi<User?> response = await userRepository.CreateAsync(user);
                if(response.Data is null) return new(null, 400, "Falha ao criar conta.");
                
                return new(null, 201, "Usuário criado com sucesso.");
            }
            catch
            {                
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<User?>> ValidatedAccessAsync(string codeAccess)
        {
            try
            {
                ResponseApi<User?> user = await userRepository.ValidatedAccessAsync(codeAccess);
                if(!user.IsSuccess) return new(null, 400, "Código inválido");
                return new(user.Data, 200, "Código de acesso confirmado");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<User?>> UpdateAsync(UpdateUserDTO request)
        {
            try
            {
                ResponseApi<User?> user = await userRepository.GetByIdAsync(request.Id);
                
                if(user.Data is null || !Validator.IsEmail(request.Email)) return new(null, 404, "Falha ao atualizar");
                if(request.ProfileUserId != user.Data.ProfileUserId)
                {
                    ResponseApi<ProfileUser?> newProfile = await profileUserRepository.GetByIdAsync(request.ProfileUserId);
                    if(newProfile.Data is null) return new(null, 404, "Perfil de usuário não encontrado");
                    user.Data.Modules = newProfile.Data.Modules;

                    user.Data.ProfileUserId = request.ProfileUserId;
                }

                if(!string.IsNullOrEmpty(request.Password))
                {
                    user.Data.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                }

                user.Data.UpdatedAt = DateTime.UtcNow;
                user.Data.Email = request.Email;
                user.Data.Name = request.Name;

                ResponseApi<User?> response = await userRepository.UpdateAsync(user.Data);
                if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

                return new(response.Data, 200, "Atualizado com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<User?>> ResendCodeAccessAsync(UpdateUserDTO request)
        {
            try
            {
                if(string.IsNullOrEmpty(request.Email)) return new(null, 400, "E-mail é obrigatório.");                    

                ResponseApi<User?> user = await userRepository.GetByEmailAsync(request.Email);

                if(user.Data is null) return new(null, 400, "E-mail inválido.");                    
                if(!Validator.IsEmail(request.Email)) return new(null, 400, "E-mail inválido.");                    

                dynamic access = Util.GenerateCodeAccess();
                string messageCode = $"Seu código de verificação é: {access.CodeAccess}";
                
                await mailHandler.SendMailAsync(request.Email, "Código de verificação", messageCode);
                
                if(user is null) return new(null, 400, "Falha ao reenviar código de acesso");

                user.Data.UpdatedAt = DateTime.UtcNow;
                user.Data.CodeAccess = access.CodeAccess;
                user.Data.CodeAccessExpiration = access.CodeAccessExpiration;
                user.Data.ValidatedAccess = false;

                ResponseApi<User?> response = await userRepository.UpdateAsync(user.Data);
                if(!response.IsSuccess) return new(null, 400, "Falha ao reenviar código de acesso");
                return new(response.Data, 200, "Novo código de acesso enviado");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<User?>> SavePhotoProfileAsync(SaveUserPhotoDTO request)
        {
            try
            {
                if (request.Photo == null || request.Photo.Length == 0) return new(null, 400, "Falha ao salvar foto de perfil");

                ResponseApi<User?> user = await userRepository.GetByIdAsync(request.Id);
                if(user.Data is null) return new(null, 404, "Falha ao salvar foto de perfil");

                string uriPhoto = await uploadHander.UploadAttachment("users", request.Photo, "/api/users/photo-profile");
                user.Data.UpdatedAt = DateTime.UtcNow;
                user.Data.Photo = uriPhoto;

                ResponseApi<User?> response = await userRepository.UpdateAsync(user.Data);
                if(!response.IsSuccess) return new(null, 400, "Falha ao salvar foto de perfil");
                return new(new () { Photo = response.Data!.Photo }, 200, "Foto de perfil salva com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<User?>> RemovePhotoProfileAsync(string id)
        {
            try
            {
                ResponseApi<User?> user = await userRepository.GetByIdAsync(id);
                if(user.Data is null) return new(null, 404, "Falha ao remover foto de perfil");
                string photo = user.Data.Photo.Split("/").Last();
                string publicId = photo.Split(".")[0];

                user.Data.UpdatedAt = DateTime.UtcNow;
                user.Data.Photo = "";

                ResponseApi<User?> response = await userRepository.UpdateAsync(user.Data);
                if(!response.IsSuccess) return new(null, 400, "Falha ao remover foto de perfil");
                return new(response.Data, 200, "Foto de perfil removida com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<User>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                ResponseApi<User> user = await userRepository.DeleteAsync(request);
                if(!user.IsSuccess) return new(null, 400, user.Message);
                return new(user.Data, 204, "Usuário excluído com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion        
    }
}