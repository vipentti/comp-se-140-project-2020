using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace E2E.Tests.APIGatewayTests
{
    public record ApiGatewayOptions
    {
        public string ApiGatewayUrl { get; init; }
    }

    public class MessagesTests : TestBase
    {
        [E2EFact]
        public async Task Messages_Returns_OK_Response()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"/messages");

            using HttpResponseMessage response = await SendRequest(request);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
