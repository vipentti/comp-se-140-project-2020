using APIGateway;
using APIGateway.Features.States;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
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
    }
}
