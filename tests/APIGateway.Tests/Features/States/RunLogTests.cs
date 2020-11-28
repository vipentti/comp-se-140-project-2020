using APIGateway.Features.States;
using Common;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TestUtils;
using Xunit;

namespace APIGateway.Tests.Features.States
{
    [CollectionDefinition("Run-Log Tests utilize state", DisableParallelization = true)]
    public abstract class RunLogTestBase
    {
        protected abstract HttpClient client { get; }
        protected abstract string endpoint { get; }
        protected abstract IDateTimeService dateTime { get; }

        [Fact]
        public virtual async Task Get_RunLog_Returns_Success_StatusCode()
        {
            // Arrange

            // Act

            var response = await client.GetAsync(endpoint);

            // Assert

            response.Should().NotBeNull();
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public virtual async Task Get_RunLog_Returns_Log()
        {
            // Arrange

            //var dateTime = new TestDateTimeService
            //{
            //    UtcNow = new System.DateTime(2020, 11, 26, 11, 30, 45),
            //};

            var state = ApplicationState.Init;

            var putResponse = await client.PutStringContent("/state", state);
            putResponse.EnsureSuccessStatusCode();

            // Act

            var response = await client.GetAsync(endpoint);

            // Assert

            response.Should().NotBeNull();
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            List<RunLogEntry> entries = content
                .Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(RunLogEntry.FromString)
                .ToList();

            entries.Should().HaveCountGreaterOrEqualTo(1);
            var last = entries.LastOrDefault();
            last.State.Should().Be(state);
            last.Timestamp.Should().BeWithin(TimeSpan.FromSeconds(5)).Before(dateTime.UtcNow);
            //entries[0].Should().Be(new RunLogEntry(dateTime.UtcNow, state));
        }

        public static async Task<IEnumerable<RunLogEntry>> GetRunLogEntries(HttpClient client, string endpoint = "/run-log")
        {
            var response = await client.GetAsync(endpoint);

            // Assert

            response.Should().NotBeNull();
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            List<RunLogEntry> entries = content
                .Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(RunLogEntry.FromString)
                .ToList();

            return entries;
        }

        [Fact]
        public virtual async Task Put_ReinitLog_Initializes_Log_Entries_And_Returns_State_Changes()
        {
            //var dateTime = new TestDateTimeService
            //{
            //    UtcNow = new System.DateTime(2020, 11, 26, 11, 30, 45),
            //};

            var clearResponse = await client.PutStringContent("/reinit-log", ApplicationState.Init);
            clearResponse.EnsureSuccessStatusCode();

            List<RunLogEntry> initialEntries = (await client.GetRunLogEntries("/run-log")).ToList();

            //initialEntries.Should().BeEmpty();

            //initialEntries.Should().SatisfyRespectively(fst =>
            //{
            //    fst.State.Should().Be(ApplicationState.Init);
            //    fst.Timestamp.Should().BeWithin(TimeSpan.FromSeconds(5)).Before(dateTime.UtcNow);
            //});

            // Initialize entries
            var states = new ApplicationState[]
            {
                ApplicationState.Init,
                ApplicationState.Running,
                ApplicationState.Paused,
                ApplicationState.Shutdown,
            };

            foreach (var state in states)
            {
                (await client.PutStringContent("/state", state)).EnsureSuccessStatusCode();
            }

            //foreach (var state in Enumeration.GetAll<ApplicationState>())
            //{
            //    var putResponse = await client.PutStringContent("/state", state);
            //    putResponse.EnsureSuccessStatusCode();
            //}

            List<RunLogEntry> newEntries = (await client.GetRunLogEntries("/run-log")).ToList();

            newEntries.Should().HaveCountGreaterThan(initialEntries.Count);

            using (new AssertionScope())
            {
                foreach (var ((state, entry), _) in states.Zip(newEntries).Indexed())
                {
                    entry.State.Should().Be(state);
                    entry.Timestamp.Should().BeWithin(TimeSpan.FromSeconds(5)).Before(dateTime.UtcNow);
                }
            }

            //newEntries.Should().SatisfyRespectively(fst =>
            //{
            //    fst.State.Should().Be(ApplicationState.Init);
            //    fst.Timestamp.Should().BeWithin(TimeSpan.FromSeconds(5)).Before(dateTime.UtcNow);
            //});
        }
    }

    public class RunLogTests : RunLogTestBase, IClassFixture<APIGatewayAppFactory>
    {
        private readonly Guid testId = System.Guid.NewGuid();
        private readonly APIGatewayAppFactory factory;
        protected override string endpoint { get; } = "/run-log";

        protected override IDateTimeService dateTime => new TestDateTimeService()
        {
            UtcNow = new System.DateTime(2020, 11, 26, 11, 30, 45),
        };

        public RunLogTests(APIGatewayAppFactory factory)
        {
            this.factory = factory;
        }

        private HttpClient _client;

        protected override HttpClient client
        {
            get
            {
                if (_client is not null)
                {
                    return _client;
                }

                _client = factory.WithTestServices(services =>
                {
                    // Services...
                    services.AddSingleton(_ => dateTime);
                }).CreateClient();

                _client.DefaultRequestHeaders.Add("X-Session-Id", testId.ToString());

                return _client;
            }
        }

        /*
        [Fact]
        public async Task Get_RunLog_Returns_Success_StatusCode()
        {
            // Arrange
            var client = factory.WithTestServices(services =>
            {
                // Services...
            }).CreateClient();

            // Act

            var response = await client.GetAsync(endpoint);

            // Assert

            response.Should().NotBeNull();
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Get_RunLog_Returns_Log()
        {
            // Arrange

            var dateTime = new TestDateTimeService
            {
                UtcNow = new System.DateTime(2020, 11, 26, 11, 30, 45),
            };

            var client = factory.WithTestServices(services =>
            {
                // Services...
                services.AddSingleton<IDateTimeService, TestDateTimeService>(_ => dateTime);
            }).CreateClient();

            var state = ApplicationState.Init;

            var putResponse = await client.PutStringContent("/state", state);
            putResponse.EnsureSuccessStatusCode();

            // Act

            var response = await client.GetAsync(endpoint);

            // Assert

            response.Should().NotBeNull();
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            List<RunLogEntry> entries = content
                .Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(RunLogEntry.FromString)
                .ToList();

            entries.Should().HaveCount(1);
            entries[0].Should().Be(new RunLogEntry(dateTime.UtcNow, state));
        }

        public static async Task<IEnumerable<RunLogEntry>> GetRunLogEntries(HttpClient client, string endpoint = "/run-log")
        {
            var response = await client.GetAsync(endpoint);

            // Assert

            response.Should().NotBeNull();
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            List<RunLogEntry> entries = content
                .Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(RunLogEntry.FromString)
                .ToList();

            return entries;
        }

        [Fact]
        public async Task Put_ReinitLog_Initializes_Log_Entries()
        {
            var dateTime = new TestDateTimeService
            {
                UtcNow = new System.DateTime(2020, 11, 26, 11, 30, 45),
            };

            var client = factory.WithTestServices(services =>
            {
                // Services...
                services.AddSingleton<IDateTimeService, TestDateTimeService>(_ => dateTime);
            }).CreateClient();

            var clearResponse = await client.PutStringContent("/reinit-log", ApplicationState.Init);
            clearResponse.EnsureSuccessStatusCode();

            List<RunLogEntry> initialEntries = (await client.GetRunLogEntries("/run-log")).ToList();

            initialEntries.Should().SatisfyRespectively(fst =>
            {
                fst.State.Should().Be(ApplicationState.Init);
                fst.Timestamp.Should().BeWithin(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow.AddHours(1));
            });

            // Initialize entries
            foreach (var state in Enumeration.GetAll<ApplicationState>())
            {
                var putResponse = await client.PutStringContent("/state", state);
                putResponse.EnsureSuccessStatusCode();
            }
        }
        */
    }
}
