using System.Threading;
using System.Threading.Tasks;

namespace APIGateway.Features.States
{
    public class StateService : IStateService
    {
        private readonly SemaphoreSlim stateSemaphore = new SemaphoreSlim(1);

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

            try
            {
                await stateSemaphore.WaitAsync();
                var previous = currentState;
                currentState = state;
                return previous;
            }
            finally
            {
                stateSemaphore.Release();
            }
        }
    }
}
