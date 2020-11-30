using StackExchange.Redis;
using System.Threading.Tasks;

namespace Common.RedisSupport
{
    public static class RedisExtensions
    {
        public static async Task<(IDatabase, ISubscriber)> GetDatabaseAndSubscriber(this IRedisClient client)
        {
            var db = await client.GetDatabase();
            var sub = await client.GetSubscriber();

            return (db, sub);
        }
    }
}
