using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIGateway.Features.States
{
    public interface IRunLogService
    {
        Task<IEnumerable<RunLogEntry>> ReinitRunLog(RunLogEntry entry);

        Task<IEnumerable<RunLogEntry>> GetRunLogEntries();

        Task ClearRunLogEntries();

        Task WriteEntry(RunLogEntry entry);

        Task WriteStateChange(RunLogEntry entry);
    }
}
