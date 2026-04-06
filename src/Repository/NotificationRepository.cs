using api_camem.src.Configuration;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace api_camem.src.Repository
{
    public class NotificationRepository(AppDbContext context) : INotificationRepository
    {
        public async Task<ResponseApi<List<dynamic>>> GetByUserIdAsync(string userId, int limit = 30)
        {
            try
            {
                BsonDocument[] pipeline =
                [
                    new("$match", new BsonDocument
                    {
                        { "userId",  userId },
                        { "deleted", false  }
                    }),
                    new("$sort",  new BsonDocument("createdAt", -1)),
                    new("$limit", limit),
                    new("$addFields", new BsonDocument { { "id", new BsonDocument("$toString", "$_id") } }),
                    new("$project", new BsonDocument { { "_id", 0 } }),
                ];

                var results = await context.Notifications.Aggregate<BsonDocument>(pipeline).ToListAsync();
                var list = results.Select(d => BsonSerializer.Deserialize<dynamic>(d)).ToList();
                return new(list, 200, "Notificações listadas com sucesso");
            }
            catch { return new(null, 500, "Erro ao buscar notificações."); }
        }

        public async Task<ResponseApi<int>> GetUnreadCountAsync(string userId)
        {
            try
            {
                long count = await context.Notifications.CountDocumentsAsync(x => x.UserId == userId && !x.Read && !x.Deleted);
                return new((int)count);
            }
            catch { return new(0); }
        }

        public async Task<ResponseApi<Notification?>> CreateAsync(Notification notification)
        {
            try
            {
                await context.Notifications.InsertOneAsync(notification);
                return new(notification, 201, "Notificação criada.");
            }
            catch { return new(null, 500, "Erro ao criar notificação."); }
        }

        public async Task<ResponseApi<Notification?>> MarkAsReadAsync(string notificationId, string userId)
        {
            try
            {
                var filter = Builders<Notification>.Filter.And(
                    Builders<Notification>.Filter.Eq(x => x.Id, notificationId),
                    Builders<Notification>.Filter.Eq(x => x.UserId, userId)
                );
                var update = Builders<Notification>.Update
                    .Set(x => x.Read,   true)
                    .Set(x => x.ReadAt, DateTime.UtcNow);

                await context.Notifications.UpdateOneAsync(filter, update);
                return new(null, 200, "Notificação marcada como lida.");
            }
            catch { return new(null, 500, "Erro ao marcar notificação."); }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var filter = Builders<Notification>.Filter.And(
                Builders<Notification>.Filter.Eq(x => x.UserId, userId),
                Builders<Notification>.Filter.Eq(x => x.Read, false)
            );
            var update = Builders<Notification>.Update
                .Set(x => x.Read,   true)
                .Set(x => x.ReadAt, DateTime.UtcNow);

            await context.Notifications.UpdateManyAsync(filter, update);
        }

        public async Task<ResponseApi<Notification>> DeleteAsync(string notificationId, string userId)
        {
            try
            {
                var notification = await context.Notifications
                    .Find(x => x.Id == notificationId && x.UserId == userId && !x.Deleted)
                    .FirstOrDefaultAsync();

                if (notification is null) return new(null, 404, "Notificação não encontrada.");

                notification.Deleted   = true;
                notification.DeletedAt = DateTime.UtcNow;

                await context.Notifications.ReplaceOneAsync(x => x.Id == notificationId, notification);
                return new(notification, 204, "Notificação excluída.");
            }
            catch { return new(null, 500, "Erro ao excluir notificação."); }
        }
    }
}