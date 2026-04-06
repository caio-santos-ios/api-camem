using System.Security.Claims;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_camem.src.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController(IUserService service) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            PaginationApi<List<dynamic>> response = await service.GetAllAsync(new(Request.Query), userId);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            ResponseApi<dynamic?> response = await service.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO user)
        {
            if (user == null) return BadRequest("Dados inválidos.");

            ResponseApi<User?> response = await service.CreateAsync(user);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserDTO user)
        {
            if (user == null) return BadRequest("Dados inválidos.");

            ResponseApi<User?> response = await service.UpdateAsync(user);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [HttpPut("code-access")]
        public async Task<IActionResult> ResendCodeAccess([FromBody] UpdateUserDTO user)
        {
            if (user == null) return BadRequest("Dados inválidos.");

            ResponseApi<User?> response = await service.ResendCodeAccessAsync(user);
            return response.IsSuccess ? Ok(new{response.Message}) : BadRequest(new{response.Message});
        }
        
        [HttpPut("confirm-access")]
        public async Task<IActionResult> ValidatedAccessAsync([FromBody] User user)
        {
            if (user == null) return BadRequest("Dados inválidos.");

            ResponseApi<User?> response = await service.ValidatedAccessAsync(user.CodeAccess);
            return response.IsSuccess ? Ok(new{response.Message}) : BadRequest(new{response.Message});
        }

        [Authorize]
        [HttpPut("profile-photo")]
        public async Task<IActionResult> SavePhotoProfileAsync([FromForm] SaveUserPhotoDTO user)
        {
            ResponseApi<User?> response = await service.SavePhotoProfileAsync(user);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpDelete("remove-profile-photo/{id}")]
        public async Task<IActionResult> RemovePhotoProfileAsync(string id)
        {
            ResponseApi<User?> response = await service.RemovePhotoProfileAsync(id);
            return response.IsSuccess ? Ok(new{response.Message}) : BadRequest(new{response.Message});
        }
        
        [Authorize]
        [HttpGet("logged")]
        public async Task<IActionResult> GetLoggedAsync()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ResponseApi<dynamic?> response = await service.GetLoggedAsync(userId!);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ResponseApi<User> response = await service.DeleteAsync(new () { Id = id, DeletedBy = userId! });
            return StatusCode(response.StatusCode, new { response.Message });
        }
    }
}