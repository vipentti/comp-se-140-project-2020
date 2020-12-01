using Common.Enumerations;
using Common.RedisSupport;
using System;
using System.Threading.Tasks;

namespace Common.States
{
    public interface IStateChangeListener
    {
        Task OnStateChange(ApplicationState state);
    }

    public interface IStateChangeListener<T>
        where T : ApplicationState
    {
        Task OnStateChange(T state);
    }

    public interface ISharedStateService : IReadonlyStateService
    {
        Task SubscribeToChanges(IStateChangeListener listener);

        Task SubscribeToChanges<T>(IStateChangeListener<T> listener)
            where T : ApplicationState;
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

        public async Task SubscribeToChanges<T>(IStateChangeListener<T> listener)
            where T : ApplicationState
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }

            var sub = await redisClient.GetSubscriber();
            var channel = await sub.SubscribeAsync(RedisStateService.StateChangeChannel);
            channel.OnMessage(async (channelMessage) =>
            {
                var applicationState = ApplicationState.FromName(channelMessage.Message);

                if (applicationState is T state)
                {
                    await listener.OnStateChange(state);
                }
            });
        }

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
