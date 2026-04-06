using Microsoft.AspNetCore.Mvc;

namespace api_camem.src.Controllers
{
    [Route("api/check")]
    [ApiController]
    public class CheckController() : ControllerBase
    {
        [HttpGet]
        public Task<IActionResult> CheckAsync()
        {
            return Task.FromResult<IActionResult>(StatusCode(200, new { Message = "API RODANDO" }));
        }
    }
}