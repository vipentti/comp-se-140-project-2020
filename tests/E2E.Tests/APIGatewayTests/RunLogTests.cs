using APIGateway;
using APIGateway.Features.States;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

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
    }
}
