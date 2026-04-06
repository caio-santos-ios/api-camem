using System.Security.Claims;
using api_camem.src.Interfaces;
using api_camem.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_camem.src.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    [Authorize]
    public class NotificationController(INotificationService service, ILoggerService loggerService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetMine([FromQuery] int limit = 30)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await service.GetByUserIdAsync(userId!, limit);
            
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await service.GetUnreadCountAsync(userId!);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        /// <summary>Endpoint para outros serviços/controllers enviarem notificações.</summary>
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] CreateNotificationDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            body.CreatedBy = userId!;

            var response = await service.SendToUserAsync(body);
            await loggerService.CreateAsync(new CreateLoggerDTO
            {
                Path = "/api/notifications/send", Method = "POST",
                Message = response.Message ?? "", StatusCode = response.StatusCode
            });
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(string id)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await service.MarkAsReadAsync(id, userId!);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await service.MarkAllAsReadAsync(userId!);
            return Ok(new { message = "Todas marcadas como lidas." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await service.DeleteAsync(id, userId!);
            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}