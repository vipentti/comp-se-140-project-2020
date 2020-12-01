using Common;
using Common.Enumerations;
using Common.States;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIGateway.Features.States
{
    public class InMemorySessionStateService : IStateService
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IDateTimeService dateTime;

        private class Services
        {
            public InMemoryStateService StateService { get; init; } = null!;
        }

        private readonly ConcurrentDictionary<string, Services> stateServices = new();

        public InMemorySessionStateService(IHttpContextAccessor contextAccessor, IDateTimeService dateTime)
        {
            this.contextAccessor = contextAccessor;
            this.stateServices.TryAdd("default", new Services()
            {
                StateService = new InMemoryStateService(new InMemoryRunLogService(), dateTime),
            });
            this.dateTime = dateTime;
        }

        public Task<ApplicationState> GetCurrentState()
        {
            var sessionId = GetSessionId();
            return GetService(sessionId).StateService.GetCurrentState();
        }

        public async Task<ApplicationState> SetCurrentState(ApplicationState state)
        {
            var sessionId = GetSessionId();

            await GetService(sessionId).StateService.SetCurrentState(state);

            return state;
        }

        private Services GetService(string sessionId)
        {
            if (!stateServices.ContainsKey(sessionId))
            {
                stateServices.TryAdd(sessionId, new Services()
                {
                    StateService = new InMemoryStateService(new InMemoryRunLogService(), dateTime),
                });
            }

            return stateServices[sessionId];
        }

        private string GetSessionId()
        {
            var context = contextAccessor.HttpContext;

            if (context is null)
            {
                return "default";
            }

            if (context.Request.Headers.TryGetValue("X-Session-Id", out var session))
            {
                return session.ToString();
            }

            return "default";
        }

        public Task<IEnumerable<RunLogEntry>> GetRunLogEntries() => GetService(GetSessionId()).StateService.GetRunLogEntries();

        public Task ClearRunLogEntries() => GetService(GetSessionId()).StateService.ClearRunLogEntries();
    }
}
