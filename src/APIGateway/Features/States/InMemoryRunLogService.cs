using Common.States;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace APIGateway.Features.States
{
    public class InMemoryRunLogService : IRunLogService
    {
        private readonly List<RunLogEntry> logEntries = new();
        private readonly SemaphoreSlim semaphoreSlim = new(1);

        public async Task ClearRunLogEntries()
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                logEntries.Clear();
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task<IEnumerable<RunLogEntry>> GetRunLogEntries()
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                return logEntries.ToList();
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task<IEnumerable<RunLogEntry>> ReinitRunLog(RunLogEntry entry)
        {
            await ClearRunLogEntries();
            await WriteEntry(entry);
            return await GetRunLogEntries();
        }

        public async Task WriteEntry(RunLogEntry entry)
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                logEntries.Add(entry);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task WriteStateChange(RunLogEntry entry)
        {
            var last = await GetLast();
            if (last?.State != entry.State)
            {
                await WriteEntry(entry);
            }
        }

        private async Task<RunLogEntry?> GetLast()
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                if (logEntries.Count == 0)
                {
                    return null;
                }

                return logEntries[logEntries.Count - 1];
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}
