using Common;
using Common.States;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.States
{
    public interface IStateService : IReadonlyStateService
    {
        Task<ApplicationState> SetCurrentState(ApplicationState state);

        Task<IEnumerable<RunLogEntry>> GetRunLogEntries();

        Task ClearRunLogEntries();
    }
}
