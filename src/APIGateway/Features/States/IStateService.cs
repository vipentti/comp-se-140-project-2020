using Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIGateway.Features.States
{
    public interface IStateService
    {
        Task<ApplicationState> GetCurrentState();

        Task<ApplicationState> SetCurrentState(ApplicationState state);

        Task<IEnumerable<RunLogEntry>> GetRunLogEntries();

        Task ClearRunLogEntries();
    }
}
