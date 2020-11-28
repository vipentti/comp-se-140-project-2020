using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace APIGateway.Features.States
{
    public class InMemoryRunLogService : IRunLogService
    {
        //private readonly ConcurrentBag<RunLogEntry> runLogEntries = new();
        //private readonly ConcurrentDictionary<int, RunLogEntry> runLog = new();
        //private int currentIndex = 0;

        private readonly List<RunLogEntry> logEntries = new();
        private readonly SemaphoreSlim semaphoreSlim = new(1);

        public async Task ClearRunLogEntries()
        {
            //runLogEntries.Clear();
            //runLog.Clear();
            //return Task.CompletedTask;
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
            //var items = runLog.ToList().OrderBy(it => it.Key).Select(it => it.Value).ToList();
            //return Task.FromResult<IEnumerable<RunLogEntry>>(items);
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
            //runLogEntries.Add(entry);
            ////var count = runLog.Count;
            //var thisIndex = Interlocked.Increment(ref currentIndex) - 1;
            ////var thisIndex = currentIndex;

            ////Interlocked.Increment(ref currentIndex);

            //runLog[thisIndex] = entry;

            ////if (!runLog.TryAdd(thisIndex - 1, entry))
            ////{
            ////    throw new InvalidOperationException($"Failed to add entry {entry}");
            ////}

            //return Task.CompletedTask;
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
