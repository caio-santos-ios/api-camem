using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_camem.src.Controllers
{
    [Route("api/chart-of-accounts")]
    [ApiController]
    public class ChartOfAccountsController(IChartOfAccountsService service) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            ResponseApi<PaginationApi<List<dynamic>>> response = await service.GetAllAsync(new(Request.Query));
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            ResponseApi<dynamic?> response = await service.GetByIdAsync(id);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ChartOfAccounts chartOfAccounts)
        {
            ResponseApi<ChartOfAccounts?> response = await service.CreateAsync(chartOfAccounts);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ChartOfAccounts chartOfAccounts)
        {
            ResponseApi<ChartOfAccounts?> response = await service.UpdateAsync(chartOfAccounts);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<ChartOfAccounts?> response = await service.DeleteAsync(id);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
    }
}