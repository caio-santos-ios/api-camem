using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_camem.src.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomerController(ICustomerService service) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            ResponseApi<PaginationApi<List<dynamic>>> response = await service.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpGet("movement")]
        public async Task<IActionResult> GetMovement()
        {
            ResponseApi<List<dynamic>> response = await service.GetMovementAsync(new(Request.Query));
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
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<Customer?> response = await service.CreateAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPost("minimal")]
        public async Task<IActionResult> CreateMinimal([FromBody] CreateCustomerMinimalDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<Customer?> response = await service.CreateMinimalAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCustomerDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<Customer?> response = await service.UpdateAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpPut("minimal")]
        public async Task<IActionResult> UpdateMinimal([FromBody] UpdateCustomerMinimalDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<Customer?> response = await service.UpdateMinimalAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<Customer> response = await service.DeleteAsync(id);

            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}