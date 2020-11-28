using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIGateway.Features.States
{
    public interface IRunLogService
    {
        Task<IEnumerable<RunLogEntry>> GetRunLogEntries();

        Task WriteEntry(RunLogEntry entry);
    }
}
