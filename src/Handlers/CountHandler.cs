using api_camem.src.Configuration;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using MongoDB.Driver;

namespace api_camem.src.Handlers
{
    public class CountHandler(AppDbContext context, ILoggerService loggerService)
    {

        public async Task<string> NextCountAsync(string collection)
        {
            try
            {
                Count? count = await context.Counts.Find(c => c.Collection == collection).FirstOrDefaultAsync();
                string nextCode = "";
                if (count == null)
                {
                    count = new Count
                    {
                        Collection = collection,
                        Index = 1
                    };
                    nextCode = "1".PadLeft(6, '0');
                    await context.Counts.InsertOneAsync(count);
                }
                else
                {
                    count.Index++;
                    var filter = Builders<Count>.Filter.Eq(c => c.Id, count.Id);
                    var update = Builders<Count>.Update.Set(c => c.Index, count.Index);
                    await context.Counts.UpdateOneAsync(filter, update);
                    nextCode = count.Index.ToString().PadLeft(6, '0');
                }

                return nextCode;
            }
            catch (Exception ex)
            {
                await loggerService.CreateAsync(new()
                {
                    Method = "COUNT_HANDLER",
                    Message = $"{ex.Message}",
                    StatusCode = 500
                });
                return "";
            }
        }
    }
}