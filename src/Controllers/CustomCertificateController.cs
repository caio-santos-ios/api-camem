using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_camem.src.Controllers
{
    [Route("api/custom-certificates")]
    [ApiController]
    public class CustomCertificateController(ICustomCertificateService service) : ControllerBase
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
        [HttpGet("last")]
        public async Task<IActionResult> GetLastAsync()
        {
            ResponseApi<dynamic?> response = await service.GetLastAsync();
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [HttpGet("validate/{keyCustomCertificate}")]
        public async Task<IActionResult> GetValidateKeyAsync(string keyCustomCertificate)
        {
            ResponseApi<dynamic?> response = await service.GetValidateKeyAsync(keyCustomCertificate);
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
        public async Task<IActionResult> Create([FromBody] CreateCustomCertificateDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<CustomCertificate?> response = await service.CreateAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCustomCertificateDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<CustomCertificate?> response = await service.UpdateAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }        
        
        [Authorize]
        [HttpPut("upload-signature")]
        public async Task<IActionResult> UploadSignature([FromForm] UploadSignatureCustomCertificateDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<string?> response = await service.UploadSignature(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }        
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<CustomCertificate> response = await service.DeleteAsync(id);

            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}