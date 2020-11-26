using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace E2E.Tests.APIGatewayTests
{
    public record ApiGatewayOptions
    {
        public string ApiGatewayUrl { get; init; }
    }

    public class MessagesTests : TestBase
    {
        private readonly ApiGatewayOptions options;

        public MessagesTests() : base()
        {
            options = Configuration.Get<ApiGatewayOptions>();
        }

        [Fact]
        public async Task Messages_Returns_OK_Response()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, options.ApiGatewayUrl);

            using HttpResponseMessage response = await SendRequest(request);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
