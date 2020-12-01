using Common;
using Common.Enumerations;
using Common.States;
using FluentAssertions;
using System;
using Xunit;

namespace APIGateway.Tests.Features.States
{
    public class RunLogEntryTests
    {
        [Fact]
        public void RunLogEntry_Supports_Round_Trip_Parsing()
        {
            var datetime = new DateTime(2020, 11, 26, 11, 30, 45, DateTimeKind.Utc);
            var entry = new RunLogEntry(
                datetime,
                ApplicationState.Init
            );

            var entryAsString = entry.ToString();

            entryAsString.Should().NotBeNullOrEmpty();
            entryAsString.Should().Be($"{datetime.ToISO8601()}: INIT");

            var parsed = RunLogEntry.FromString(entryAsString);

            parsed.Should().NotBeNull();
            parsed.Should().Be(entry);
        }
    }
}
