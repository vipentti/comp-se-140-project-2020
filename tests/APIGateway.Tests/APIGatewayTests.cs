using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace APIGateway.Tests
{
    public class APIGatewayTests : IClassFixture<APIGatewayAppFactory>
    {
        private readonly APIGatewayAppFactory factory;

        public APIGatewayTests(APIGatewayAppFactory factory)
        {
            this.factory = factory;
        }

        [Theory]
        [InlineData("/messages")]
        [InlineData("/state")]
        [InlineData("/run-log")]
        [InlineData("/node-statistic")]
        [InlineData("/queue-statistic")]
        public async Task Get_Endpoint_Returns_Success_StatusCode(string endpoint)
        {
            // Arrange
            var client = factory.CreateClient();

            // Act

            var response = await client.GetAsync(endpoint);

            // Assert

            response.Should().NotBeNull();
            response.EnsureSuccessStatusCode();
        }
    }
}
