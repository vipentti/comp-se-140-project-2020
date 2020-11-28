using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIGateway.Features.States
{
    public class InMemoryRunLogService : IRunLogService
    {
        private readonly ConcurrentBag<RunLogEntry> runLogEntries = new();
        private readonly ConcurrentDictionary<int, RunLogEntry> runLog = new();

        public Task ClearRunLogEntries()
        {
            runLogEntries.Clear();
            runLog.Clear();
            return Task.CompletedTask;
        }

        public Task<IEnumerable<RunLogEntry>> GetRunLogEntries()
        {
            var items = runLog.ToList().OrderBy(it => it.Key).Select(it => it.Value).ToList();
            return Task.FromResult<IEnumerable<RunLogEntry>>(items);
        }

        public Task<IEnumerable<RunLogEntry>> ReinitRunLog(RunLogEntry entry)
        {
            ClearRunLogEntries();
            WriteEntry(entry);
            return GetRunLogEntries();
        }

        public Task WriteEntry(RunLogEntry entry)
        {
            runLogEntries.Add(entry);
            var count = runLog.Count;
            runLog.TryAdd(count, entry);
            return Task.CompletedTask;
        }
    }
}
