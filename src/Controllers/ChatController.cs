using System.Security.Claims;
using api_camem.src.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_camem.src.Controllers
{
    [Route("api/chat")]
    [ApiController]
    [Authorize]
    public class ChatController(IChatService service) : ControllerBase
    {
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await service.GetConversationsAsync(userId!);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [HttpGet("messages/{conversationId}")]
        public async Task<IActionResult> GetMessages(string conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var response = await service.GetMessagesAsync(conversationId, page, pageSize);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await service.GetUnreadCountAsync(userId!);
            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}