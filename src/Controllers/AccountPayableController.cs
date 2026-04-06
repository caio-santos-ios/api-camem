using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_camem.src.Controllers
{
    [Route("api/accounts-payable")]
    [ApiController]
    public class AccountPayableController(IAccountPayableService service) : ControllerBase
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
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAccountPayableDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");
            ResponseApi<AccountPayable?> response = await service.CreateAsync(body);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateAccountPayableDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");
            ResponseApi<AccountPayable?> response = await service.UpdateAsync(body);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpPut("pay")]
        public async Task<IActionResult> Pay([FromBody] PayAccountPayableDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");
            ResponseApi<AccountPayable?> response = await service.PayAsync(body);
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPut("cancel")]
        public async Task<IActionResult> Cancel([FromBody] CancelAccountPayableDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");
            ResponseApi<AccountPayable?> response = await service.CancelAsync(body);
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<AccountPayable> response = await service.DeleteAsync(id);
            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}
