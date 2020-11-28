using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIGateway.Features.States
{
    public class InMemoryRunLogService : IRunLogService
    {
        private readonly ConcurrentBag<RunLogEntry> runLogEntries = new();

        public Task<IEnumerable<RunLogEntry>> GetRunLogEntries() => Task.FromResult<IEnumerable<RunLogEntry>>(runLogEntries.ToList());

        public Task WriteEntry(RunLogEntry entry)
        {
            runLogEntries.Add(entry);
            return Task.CompletedTask;
        }
    }
}
