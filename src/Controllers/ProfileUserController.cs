using System.Security.Claims;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_camem.src.Controllers
{
    [Route("api/profile-users")]
    [ApiController]
    public class ProfileUserController(IProfileUserService service) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await service.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            ResponseApi<dynamic?> response = await service.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpGet("select")]
        public async Task<IActionResult> GetSelect()
        {
            ResponseApi<List<dynamic>> response = await service.GetSelectAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProfileUserDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<ProfileUser?> response = await service.CreateAsync(body);
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateProfileUserDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<ProfileUser?> response = await service.UpdateAsync(body);
            return StatusCode(response.StatusCode, new { response.Result });
        }    

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ResponseApi<ProfileUser> response = await service.DeleteAsync(new () { Id = id, DeletedBy = userId! });
            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}