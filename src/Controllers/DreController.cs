using api_camem.src.Interfaces;
using api_camem.src.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_camem.src.Controllers
{
    [Route("api/dre")]
    [ApiController]
    public class DreController(IDreService service) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Generate(
            [FromQuery] string startDate,
            [FromQuery] string endDate,
            [FromQuery] string regime = "competencia"
        )
        {
            if (!DateTime.TryParse(startDate, out DateTime start))
            {
                return BadRequest(new { message = "Data inicial inválida" });
            }

            if (!DateTime.TryParse(endDate, out DateTime end))
            {
                return BadRequest(new { message = "Data final inválida" });
            }

            ResponseApi<dynamic?> response = await service.GenerateAsync(start, end, regime);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
    }
}