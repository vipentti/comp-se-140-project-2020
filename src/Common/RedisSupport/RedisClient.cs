using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Common.RedisSupport
{
    public interface IRedisClient
    {
        Task<IDatabase> GetDatabase();

        Task<ISubscriber> GetSubscriber();
    }

    public class RedisClient : IRedisClient
    {
        private readonly RedisOptions redisOptions;

        private ConnectionMultiplexer? connectionMultiplexer;

        public RedisClient(IOptions<RedisOptions> redisOptions)
        {
            this.redisOptions = redisOptions.Value;
        }

        public async Task<IDatabase> GetDatabase()
        {
            return (await EnsureConnected()).GetDatabase();
        }

        public async Task<ISubscriber> GetSubscriber() => (await EnsureConnected()).GetSubscriber();

        private async Task<ConnectionMultiplexer> EnsureConnected()
        {
            if (connectionMultiplexer is null)
            {
                await Connect();
            }

            if (connectionMultiplexer is null)
            {
                throw new InvalidOperationException("Failed to connect to redis");
            }

            return connectionMultiplexer;
        }

        private async Task Connect()
        {
            connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(new ConfigurationOptions()
            {
                EndPoints =
                {
                    { redisOptions.Host, redisOptions.Port }
                },
            });
        }
    }
}
