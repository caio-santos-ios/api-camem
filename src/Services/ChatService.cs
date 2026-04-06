using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;

namespace api_camem.src.Services
{
    public class ChatService(IChatRepository repository) : IChatService
    {
        public async Task<ResponseApi<List<dynamic>>> GetConversationsAsync(string userId)
            => await repository.GetConversationsAsync(userId);

        public async Task<ResponseApi<List<dynamic>>> GetMessagesAsync(string conversationId, int page = 1, int pageSize = 50)
            => await repository.GetMessagesAsync(conversationId, page, pageSize);

        public async Task<ResponseApi<ChatMessage?>> SendMessageAsync(SendChatMessageDTO dto)
        {
            try
            {
                // Garante que a conversa existe
                string convId = string.Join("_", new[] { dto.SenderId, dto.ReceiverId }.OrderBy(x => x));
                await repository.GetOrCreateConversationAsync(dto.SenderId, dto.ReceiverId);

                var message = new ChatMessage
                {
                    ConversationId = convId,
                    SenderId       = dto.SenderId,
                    SenderName     = dto.SenderName,
                    SenderPhoto    = dto.SenderPhoto,
                    ReceiverId     = dto.ReceiverId,
                    Content        = dto.Content,
                };

                var response = await repository.CreateMessageAsync(message);
                if (!response.IsSuccess || response.Data is null)
                    return new(null, 500, "Falha ao salvar mensagem.");

                await repository.UpdateConversationLastMessageAsync(convId, dto.Content);

                return response;
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado.");
            }
        }

        public async Task MarkConversationAsReadAsync(string conversationId, string userId)
            => await repository.MarkConversationAsReadAsync(conversationId, userId);

        public async Task<ResponseApi<int>> GetUnreadCountAsync(string userId)
            => await repository.GetUnreadCountAsync(userId);
    }
}