using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;

namespace api_camem.src.Interfaces
{
    public interface IProfileUserService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request);
        Task<ResponseApi<ProfileUser?>> CreateAsync(CreateProfileUserDTO request);
        Task<ResponseApi<ProfileUser?>> UpdateAsync(UpdateProfileUserDTO request);
        Task<ResponseApi<ProfileUser>> DeleteAsync(DeleteDTO request);
    }
}