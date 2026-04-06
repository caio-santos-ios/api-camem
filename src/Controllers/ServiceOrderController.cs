using System.Security.Claims;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_camem.src.Controllers
{
    [Route("api/service-orders")]
    [ApiController]
    public class ServiceOrderController(IServiceOrderService service) : ControllerBase
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
        [HttpGet("warranty-check")]
        public async Task<IActionResult> WarrantyCheck([FromQuery] string? customerId, [FromQuery] string? serialImei)
        {
            ResponseApi<dynamic?> response = await service.CheckWarrantyAsync(customerId, serialImei);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServiceOrderDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");
            body.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            ResponseApi<ServiceOrder?> response = await service.CreateAsync(body);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateServiceOrderDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");
            ResponseApi<ServiceOrder?> response = await service.UpdateAsync(body);
            body.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPut("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusServiceOrderDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");
            ResponseApi<ServiceOrder?> response = await service.UpdateStatusAsync(body);
            body.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpPut("close")]
        public async Task<IActionResult> Close([FromBody] CloseServiceOrderDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");
            ResponseApi<ServiceOrder?> response = await service.CloseAsync(body);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<ServiceOrder> response = await service.DeleteAsync(id);
            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}