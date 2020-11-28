using APIGateway.Features.States;
using Common;
using FluentAssertions;
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
    public class RunLogTests : IClassFixture<APIGatewayAppFactory>
    {
        private readonly APIGatewayAppFactory factory;
        private readonly string endpoint = "/run-log";

        public RunLogTests(APIGatewayAppFactory factory)
        {
            this.factory = factory;
        }

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
    }
}
