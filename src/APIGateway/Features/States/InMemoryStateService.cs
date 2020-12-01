using Common;
using Common.Enumerations;
using Common.States;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace APIGateway.Features.States
{
    public class InMemoryStateService : IStateService
    {
        private readonly SemaphoreSlim stateSemaphore = new SemaphoreSlim(1);
        private readonly IRunLogService runLog;
        private readonly IDateTimeService dateTime;

        public InMemoryStateService(IRunLogService runLog, IDateTimeService dateTime)
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

            await runLog.WriteStateChange(new RunLogEntry(dateTime.UtcNow, state));

            return previous;
        }

        public async Task<IEnumerable<RunLogEntry>> GetRunLogEntries() => await runLog.GetRunLogEntries();

        public async Task ClearRunLogEntries() => await runLog.ClearRunLogEntries();
    }
}
