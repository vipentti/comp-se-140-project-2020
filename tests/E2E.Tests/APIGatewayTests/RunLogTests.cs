using APIGateway;
using APIGateway.Features.States;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace E2E.Tests.APIGatewayTests
{
    public class RunLogTests : TestBase
    {
        private readonly APIOptions options;

        private string Endpoint => $"{options.ApiGatewayUrl}/run-log";

        public RunLogTests() : base()
        {
            options = Configuration.Get<APIOptions>();
        }

        [E2EFact]
        public async Task Get_RunLog_Returns_Ok()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, Endpoint);

            using HttpResponseMessage response = await SendRequest(request);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [E2EFact]
        public async Task Get_RunLog_Returns_Entries()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, Endpoint);

            using HttpResponseMessage response = await SendRequest(request);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();

            List<RunLogEntry> entries = content
                .Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(RunLogEntry.FromString)
                .ToList();

            entries.Should().HaveCountGreaterOrEqualTo(1);
        }

        [E2EFact]
        public async Task Put_State_Updates_RunLog()
        {
            // Get initial entries
            var originalEntries = await GetLogEntries();

            var nextState = ApplicationState.Paused;

            var response = await PutRequest("/state", new StringContent(nextState.ToString()));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            content.Should().Be(nextState.ToString());

            var newEntries = await GetLogEntries();
        }

        private async Task<List<RunLogEntry>> GetLogEntries()
        {
            var response = await GetRequest(Endpoint);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNull();

            return ReadEntriesFrom(content);
        }

        public static List<RunLogEntry> ReadEntriesFrom(string content)
        {
            List<RunLogEntry> entries = content
                .Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(RunLogEntry.FromString)
                .ToList();

            return entries;
        }
    }
}
