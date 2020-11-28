using Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIGateway.Features.States
{
    public class SessionStateService : IStateService
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IDateTimeService dateTime;

        private class Services
        {
            public StateService StateService { get; init; } = new StateService();
            public InMemoryRunLogService RunLog { get; init; } = new InMemoryRunLogService();
        }

        private readonly ConcurrentDictionary<string, Services> stateServices = new();

        public SessionStateService(IHttpContextAccessor contextAccessor, IDateTimeService dateTime)
        {
            this.contextAccessor = contextAccessor;
            this.stateServices.TryAdd("default", new Services());
            this.dateTime = dateTime;
        }

        public Task<ApplicationState> GetCurrentState()
        {
            var sessionId = GetSessionId();

            //if (!stateServices.ContainsKey(sessionId))
            //{
            //    stateServices.TryAdd(sessionId, new StateService());
            //}

            //return stateServices[sessionId].GetCurrentState();
            return GetService(sessionId).StateService.GetCurrentState();
        }

        public async Task<ApplicationState> SetCurrentState(ApplicationState state)
        {
            var sessionId = GetSessionId();

            await GetService(sessionId).StateService.SetCurrentState(state);
            await GetService(sessionId).RunLog.WriteStateChange(new RunLogEntry(dateTime.UtcNow, state));

            return state;
        }

        private Services GetService(string sessionId)
        {
            if (!stateServices.ContainsKey(sessionId))
            {
                stateServices.TryAdd(sessionId, new Services());
            }

            return stateServices[sessionId];
        }

        private string GetSessionId()
        {
            var context = contextAccessor.HttpContext;

            if (context is null)
            {
                return "default";
                //throw new ArgumentNullException(nameof(context));
            }

            if (context.Request.Headers.TryGetValue("X-Session-Id", out var session))
            {
                return session.ToString();
            }

            return "default";
            //throw new InvalidOperationException("Missing X-Session-Id header");
        }
    }
}
