using APIGateway.Features.Original;
using APIGateway.Features.States;
using Common;
using Common.States;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Moq;
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
        }

        [Fact]
        public virtual async Task Put_ReinitLog_Initializes_Log_Entries_And_Returns_State_Changes()
        {
            var clearResponse = await client.PutStringContent("/reinit-log", ApplicationState.Init);
            clearResponse.EnsureSuccessStatusCode();

            List<RunLogEntry> initialEntries = (await client.GetRunLogEntries("/run-log")).ToList();

            initialEntries.Should().SatisfyRespectively(fst =>
            {
                fst.State.Should().Be(ApplicationState.Init);
                fst.Timestamp.Should().BeWithin(TimeSpan.FromSeconds(5)).Before(dateTime.UtcNow);
            });

            // Initialize entries
            var states = new ApplicationState[]
            {
                ApplicationState.Init,
                ApplicationState.Running,
                ApplicationState.Paused,
                ApplicationState.Running,
            };

            foreach (var state in states)
            {
                (await client.PutStringContent("/state", state)).EnsureSuccessStatusCode();
            }

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

        private readonly Mock<IOriginalService> originalServiceMock;

        public RunLogTests(APIGatewayAppFactory factory)
        {
            this.factory = factory;
            this.originalServiceMock = Extensions.CreateMockOriginalService();
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
                    services.SetupMockServices(originalServiceMock);
                    services.AddSingleton(_ => dateTime);
                }).CreateClient();

                _client.DefaultRequestHeaders.Add("X-Session-Id", testId.ToString());

                return _client;
            }
        }
    }
}
