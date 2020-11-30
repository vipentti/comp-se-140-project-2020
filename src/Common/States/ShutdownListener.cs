using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Common.States
{
    public class ShutdownListener : BackgroundService, IStateChangeListener
    {
        private readonly ILogger<ShutdownListener> logger;
        private readonly ISharedStateService sharedState;
        private readonly IHostApplicationLifetime applicationLifetime;

        public ShutdownListener(ILogger<ShutdownListener> logger, ISharedStateService sharedState, IHostApplicationLifetime applicationLifetime)
        {
            this.logger = logger;
            this.sharedState = sharedState;
            this.applicationLifetime = applicationLifetime;
        }

        public async Task OnStateChange(ApplicationState state)
        {
            if (state == ApplicationState.Shutdown)
            {
                logger.LogInformation("Received {State}. Shutting down.", state);
                await Task.Delay(100);
                applicationLifetime.StopApplication();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) => await sharedState.SubscribeToChanges(this);
    }
}
