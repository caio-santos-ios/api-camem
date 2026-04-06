using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_camem.src.Controllers
{
    [Route("api/accounts-receivable")]
    [ApiController]
    public class AccountReceivableController(IAccountReceivableService service) : ControllerBase
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
        public async Task<IActionResult> GetById(string id)
        {
            ResponseApi<dynamic?> response = await service.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAccountReceivableDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");
            ResponseApi<AccountReceivable?> response = await service.CreateAsync(body);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateAccountReceivableDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");
            ResponseApi<AccountReceivable?> response = await service.UpdateAsync(body);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpPut("pay")]
        public async Task<IActionResult> Pay([FromBody] PayAccountReceivableDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");
            ResponseApi<AccountReceivable?> response = await service.PayAsync(body);
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPut("cancel")]
        public async Task<IActionResult> Cancel([FromBody] CancelAccountReceivableDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");
            ResponseApi<AccountReceivable?> response = await service.CancelAsync(body);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<AccountReceivable> response = await service.DeleteAsync(id);
            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}
