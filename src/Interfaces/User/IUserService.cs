using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;

namespace api_camem.src.Interfaces
{
    public interface IUserService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<List<dynamic>>> GetCountAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<dynamic?>> GetLoggedAsync(string id);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request);
        Task<ResponseApi<User?>> CreateAsync(CreateUserDTO request);
        Task<ResponseApi<User?>> UpdateAsync(UpdateUserDTO request);
        Task<ResponseApi<User?>> UpdateStatusAccessAsync(UpdateUserStatusAccessDTO request);
        Task<ResponseApi<User?>> UpdateSettingNotificationAsync(UpdateUserSettingNotificationDTO request);
        Task<ResponseApi<User?>> SavePhotoProfileAsync(SaveUserPhotoDTO request);
        Task<ResponseApi<User?>> ResendCodeAccessAsync(UpdateUserDTO request);
        Task<ResponseApi<User?>> RemovePhotoProfileAsync(string id);
        Task<ResponseApi<User?>> ValidatedAccessAsync(string codeAccess);
        Task<ResponseApi<User>> DeleteAsync(DeleteDTO request);
    }
}