using Common;
using System;

namespace Common.States
{
    public record RunLogEntry(DateTime Timestamp, ApplicationState State)
    {
        public static RunLogEntry FromString(string input)
        {
            var parts = input.Split(": ", StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                throw new InvalidOperationException($"'{input}' is not valid {nameof(RunLogEntry)}");
            }

            var datetime = parts[0].FromISO8601() ?? throw new InvalidOperationException($"Invalid datetime {parts[0]}");

            return new RunLogEntry(
                datetime,
                ApplicationState.FromName(parts[1]) ?? throw new InvalidOperationException($"Invalid {nameof(ApplicationState)} {parts[1]}")
            );
        }

        public override string ToString() => $"{Timestamp.ToISO8601()}: {State}";
    }
}
