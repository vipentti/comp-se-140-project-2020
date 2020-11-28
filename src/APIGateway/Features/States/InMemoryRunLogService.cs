using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIGateway.Features.States
{
    public class InMemoryRunLogService : IRunLogService
    {
        private readonly ConcurrentBag<RunLogEntry> runLogEntries = new();

        public Task ClearRunLogEntries()
        {
            runLogEntries.Clear();
            return Task.CompletedTask;
        }

        public Task<IEnumerable<RunLogEntry>> GetRunLogEntries() => Task.FromResult<IEnumerable<RunLogEntry>>(runLogEntries.ToList());

        public Task<IEnumerable<RunLogEntry>> ReinitRunLog(RunLogEntry entry)
        {
            runLogEntries.Clear();
            runLogEntries.Add(entry);
            return GetRunLogEntries();
        }

        public Task WriteEntry(RunLogEntry entry)
        {
            runLogEntries.Add(entry);
            return Task.CompletedTask;
        }
    }
}
