using Common;
using System.Threading;
using System.Threading.Tasks;

namespace APIGateway.Features.States
{
    public class StateService : IStateService
    {
        private readonly SemaphoreSlim stateSemaphore = new SemaphoreSlim(1);
        private readonly IRunLogService runLog;
        private readonly IDateTimeService dateTime;

        public StateService(IRunLogService runLog, IDateTimeService dateTime)
        {
            this.runLog = runLog;
            this.dateTime = dateTime;
        }

        private ApplicationState currentState = ApplicationState.Init;

        public async Task<ApplicationState> GetCurrentState()
        {
            try
            {
                await stateSemaphore.WaitAsync();
                return currentState;
            }
            finally
            {
                stateSemaphore.Release();
            }
        }

        public async Task<ApplicationState> SetCurrentState(ApplicationState state)
        {
            if (state is null)
            {
                throw new System.ArgumentNullException(nameof(state));
            }

            ApplicationState previous;

            try
            {
                await stateSemaphore.WaitAsync();
                previous = currentState;
                currentState = state;
            }
            finally
            {
                stateSemaphore.Release();
            }

            await runLog.WriteEntry(new RunLogEntry(dateTime.UtcNow, currentState));

            return previous;
        }
    }
}
