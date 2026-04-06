using api_camem.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_camem.src.Models
{
    public class ChatMessage : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        /// <summary>ID da conversa (pode ser userId_userId ordenado ou um groupId)</summary>
        [BsonElement("conversationId")]
        public string ConversationId { get; set; } = string.Empty;

        [BsonElement("senderId")]
        public string SenderId { get; set; } = string.Empty;

        [BsonElement("senderName")]
        public string SenderName { get; set; } = string.Empty;

        [BsonElement("senderPhoto")]
        public string SenderPhoto { get; set; } = string.Empty;

        [BsonElement("receiverId")]
        public string ReceiverId { get; set; } = string.Empty;

        [BsonElement("content")]
        public string Content { get; set; } = string.Empty;

        [BsonElement("read")]
        public bool Read { get; set; } = false;

        [BsonElement("readAt")]
        public DateTime? ReadAt { get; set; }
    }

    public class Conversation : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("conversationId")]
        public string ConversationId { get; set; } = string.Empty;

        [BsonElement("participants")]
        public List<string> Participants { get; set; } = [];

        [BsonElement("lastMessage")]
        public string LastMessage { get; set; } = string.Empty;

        [BsonElement("lastMessageAt")]
        public DateTime? LastMessageAt { get; set; }

        [BsonElement("unreadCount")]
        public int UnreadCount { get; set; } = 0;
    }
}