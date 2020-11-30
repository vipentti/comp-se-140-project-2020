using Common.RedisSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.States
{
    public interface IStateChangeListener
    {
        Task OnStateChange(ApplicationState state);
    }

    public interface ISharedStateService : IReadonlyStateService
    {
        Task SubscribeToChanges(IStateChangeListener listener);
    }

    public class SharedStateService : ISharedStateService
    {
        private readonly IReadonlyStateService readonlyState;
        private readonly IRedisClient redisClient;

        public SharedStateService(IReadonlyStateService readonlyState, IRedisClient redisClient)
        {
            this.readonlyState = readonlyState;
            this.redisClient = redisClient;
        }

        public async Task<ApplicationState> GetCurrentState() => await readonlyState.GetCurrentState();

        public async Task SubscribeToChanges(IStateChangeListener listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }

            var sub = await redisClient.GetSubscriber();
            var channel = await sub.SubscribeAsync(RedisStateService.StateChangeChannel);
            channel.OnMessage(async (channelMessage) =>
            {
                var state = ApplicationState.FromName(channelMessage.Message);

                if (state is not null)
                {
                    await listener.OnStateChange(state);
                }
            });
        }
    }
}
