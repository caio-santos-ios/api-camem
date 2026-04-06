using api_camem.src.Configuration;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace api_camem.src.Repository
{
    public class ChatRepository(AppDbContext context) : IChatRepository
    {
        private static string BuildConversationId(string a, string b)
        {
            var sorted = new[] { a, b }.OrderBy(x => x).ToArray();
            return $"{sorted[0]}_{sorted[1]}";
        }

        public async Task<ResponseApi<Conversation?>> GetOrCreateConversationAsync(string userIdA, string userIdB)
        {
            try
            {
                string convId = BuildConversationId(userIdA, userIdB);
                var existing  = await context.Conversations
                    .Find(x => x.ConversationId == convId && !x.Deleted)
                    .FirstOrDefaultAsync();

                if (existing is not null) return new(existing);

                var conversation = new Conversation
                {
                    ConversationId = convId,
                    Participants   = [userIdA, userIdB],
                };
                await context.Conversations.InsertOneAsync(conversation);
                return new(conversation, 201, "Conversa criada.");
            }
            catch { return new(null, 500, "Erro ao buscar/criar conversa."); }
        }

        public async Task<ResponseApi<List<dynamic>>> GetConversationsAsync(string userId)
        {
            try
            {
                BsonDocument[] pipeline =
                [
                    new("$match", new BsonDocument
                    {
                        { "participants", userId },
                        { "deleted",      false   }
                    }),
                    new("$sort",  new BsonDocument("lastMessageAt", -1)),
                    new("$addFields", new BsonDocument { { "id", new BsonDocument("$toString", "$_id") } }),
                    new("$project", new BsonDocument { { "_id", 0 } }),
                ];

                var results = await context.Conversations.Aggregate<BsonDocument>(pipeline).ToListAsync();
                var list    = results.Select(d => BsonSerializer.Deserialize<dynamic>(d)).ToList();
                return new(list);
            }
            catch { return new(null, 500, "Erro ao buscar conversas."); }
        }

        public async Task<ResponseApi<List<dynamic>>> GetMessagesAsync(string conversationId, int page = 1, int pageSize = 50)
        {
            try
            {
                int skip = pageSize * (page - 1);
                BsonDocument[] pipeline =
                [
                    new("$match", new BsonDocument
                    {
                        { "conversationId", conversationId },
                        { "deleted",        false           }
                    }),
                    new("$sort",  new BsonDocument("createdAt", -1)),
                    new("$skip",  skip),
                    new("$limit", pageSize),
                    new("$sort",  new BsonDocument("createdAt", 1)),
                    new("$addFields", new BsonDocument { { "id", new BsonDocument("$toString", "$_id") } }),
                    new("$project", new BsonDocument { { "_id", 0 } }),
                ];

                var results = await context.ChatMessages.Aggregate<BsonDocument>(pipeline).ToListAsync();
                var list    = results.Select(d => BsonSerializer.Deserialize<dynamic>(d)).ToList();
                return new(list);
            }
            catch { return new(null, 500, "Erro ao buscar mensagens."); }
        }

        public async Task<ResponseApi<ChatMessage?>> CreateMessageAsync(ChatMessage message)
        {
            try
            {
                await context.ChatMessages.InsertOneAsync(message);
                return new(message, 201, "Mensagem enviada.");
            }
            catch { return new(null, 500, "Erro ao enviar mensagem."); }
        }

        public async Task MarkConversationAsReadAsync(string conversationId, string userId)
        {
            var msgFilter = Builders<ChatMessage>.Filter.And(
                Builders<ChatMessage>.Filter.Eq(x => x.ConversationId, conversationId),
                Builders<ChatMessage>.Filter.Eq(x => x.ReceiverId,     userId),
                Builders<ChatMessage>.Filter.Eq(x => x.Read,           false)
            );
            var msgUpdate = Builders<ChatMessage>.Update
                .Set(x => x.Read,   true)
                .Set(x => x.ReadAt, DateTime.UtcNow);

            await context.ChatMessages.UpdateManyAsync(msgFilter, msgUpdate);

            var convFilter = Builders<Conversation>.Filter.Eq(x => x.ConversationId, conversationId);
            var convUpdate = Builders<Conversation>.Update.Set(x => x.UnreadCount, 0);
            await context.Conversations.UpdateOneAsync(convFilter, convUpdate);
        }

        public async Task<ResponseApi<int>> GetUnreadCountAsync(string userId)
        {
            try
            {
                long count = await context.ChatMessages
                    .CountDocumentsAsync(x => x.ReceiverId == userId && !x.Read && !x.Deleted);
                return new((int)count);
            }
            catch { return new(0); }
        }

        public async Task UpdateConversationLastMessageAsync(string conversationId, string lastMessage)
        {
            var filter = Builders<Conversation>.Filter.Eq(x => x.ConversationId, conversationId);
            var update = Builders<Conversation>.Update
                .Set(x => x.LastMessage,   lastMessage)
                .Set(x => x.LastMessageAt, DateTime.UtcNow);

            await context.Conversations.UpdateOneAsync(filter, update);
        }
    }
}