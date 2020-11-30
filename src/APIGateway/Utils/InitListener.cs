﻿using Common;
using Common.States;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace APIGateway.Utils
{
    public class InitListener : BackgroundService, IStateChangeListener
    {
        private readonly ILogger<InitListener> logger;
        private readonly ISharedStateService sharedState;
        private readonly IStateService stateService;

        public InitListener(ILogger<InitListener> logger, ISharedStateService sharedState, IStateService stateService)
        {
            this.logger = logger;
            this.sharedState = sharedState;
            this.stateService = stateService;
        }

        public async Task OnStateChange(ApplicationState state)
        {
            logger.LogInformation("Received {State}", state);
            if (state == ApplicationState.Init)
            {
                await Task.Delay(0);
                await stateService.SetCurrentState(ApplicationState.Running);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) => await sharedState.SubscribeToChanges(this);
    }
}
