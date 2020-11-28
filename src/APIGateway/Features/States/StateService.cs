using System.Threading.Tasks;

namespace APIGateway.Features.States
{
    public class StateService : IStateService
    {
        private ApplicationState currentState = ApplicationState.Init;

        public Task<ApplicationState> GetCurrentState()
        {
            return Task.FromResult(currentState);
        }

        public Task<ApplicationState> SetCurrentState(ApplicationState state)
        {
            var previous = currentState;

            currentState = state;

            return Task.FromResult(previous);
        }
    }
}
