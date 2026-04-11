using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Services;
using api_camem.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_camem.src.Controllers
{
    [Route("api/certificates")]
    [ApiController]
    public class CertificateController(CertificateService service) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            ResponseApi<PaginationApi<List<dynamic>>> response = await service.GetAllAsync(new(Request.Query));
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
        public async Task<IActionResult> Create([FromBody] CreateCertificateDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<Certificate?> response = await service.CreateAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCertificateDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<Certificate?> response = await service.UpdateAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }        
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<Certificate> response = await service.DeleteAsync(id);

            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}