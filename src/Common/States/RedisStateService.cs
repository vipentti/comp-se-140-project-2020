using Common.RedisSupport;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.States
{
    public class RedisStateService : IStateService
    {
        public const string StateKey = "application-state";
        public const string RunLogKey = "run-log";
        public const string StateChangeChannel = "state-change";

        private readonly IRedisClient redisClient;
        private readonly ISessionService sessionService;
        private readonly IDateTimeService dateTime;

        public RedisStateService(IRedisClient redisClient, IDateTimeService dateTime, ISessionService sessionService)
        {
            this.redisClient = redisClient;
            this.dateTime = dateTime;
            this.sessionService = sessionService;
        }

        private string CurrentStateKey => $"{StateKey}:{sessionService.SessionId}";
        private string CurrentRunLogKey => $"{RunLogKey}:{sessionService.SessionId}";

        public async Task ClearRunLogEntries()
        {
            var db = await redisClient.GetDatabase();

            await db.ListTrimAsync(CurrentRunLogKey, 0, -1);
        }

        public async Task<ApplicationState> GetCurrentState()
        {
            var db = await redisClient.GetDatabase();

            var state = await db.StringGetAsync(CurrentStateKey);

            return ApplicationState.FromName(state) ?? ApplicationState.Unknown;
        }

        public async Task<IEnumerable<RunLogEntry>> GetRunLogEntries()
        {
            var db = await redisClient.GetDatabase();

            StackExchange.Redis.RedisValue[] entries = await db.ListRangeAsync(CurrentRunLogKey);

            return entries.Select(it => it.ToString()).Select(RunLogEntry.FromString);
        }

        public async Task<ApplicationState> SetCurrentState(ApplicationState state)
        {
            var db = await redisClient.GetDatabase();

            var currenState = await GetCurrentState();
            await db.StringSetAsync(CurrentStateKey, state.ToString());

            if (currenState != state)
            {
                await db.ListRightPushAsync(CurrentRunLogKey, new StackExchange.Redis.RedisValue[] { new RunLogEntry(dateTime.UtcNow, state).ToString() });

                var sub = await redisClient.GetSubscriber();

                await sub.PublishAsync(StateChangeChannel, state.ToString());
            }

            return state;
        }
    }
}
