using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_camem.src.Controllers
{
    [Route("api/suppliers")]
    [ApiController]
    public class SupplierController(ISupplierService service) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            ResponseApi<PaginationApi<List<dynamic>>> response = await service.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpGet("autocomplete")]
        public async Task<IActionResult> GetAutocompleteAsync()
        {
            ResponseApi<List<dynamic>> response = await service.GetAutocompleteAsync(new(Request.Query));
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
        public async Task<IActionResult> Create([FromBody] CreateSupplierDTO supplier)
        {
            if (supplier == null) return BadRequest("Dados inválidos.");

            ResponseApi<Supplier?> response = await service.CreateAsync(supplier);

            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpPost("minimal")]
        public async Task<IActionResult> CreateMinimal([FromBody] CreateSupplierMinimalDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<Supplier?> response = await service.CreateMinimalAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateSupplierDTO supplier)
        {
            if (supplier == null) return BadRequest("Dados inválidos.");

            ResponseApi<Supplier?> response = await service.UpdateAsync(supplier);

            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<Supplier> response = await service.DeleteAsync(id);

            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}