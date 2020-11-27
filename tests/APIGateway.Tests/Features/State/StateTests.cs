using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using APIGateway.Features.Messages;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace APIGateway.Tests.Features.Messages
{
    public class StateTests : IClassFixture<APIGatewayAppFactory>
    {
        private readonly APIGatewayAppFactory factory;
        private readonly string endpoint = "/state";

        public StateTests(APIGatewayAppFactory factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task Get_State_Returns_Current_State()
        {
            // Arrange
            var client = factory.WithTestServices(services=> {
                // Services...
            }).CreateClient();

            // Act

            var response = await client.GetAsync(endpoint);

            // Assert

            response.Should().NotBeNull();
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            content.Should().Be("INIT");
        }

        [Fact]
        public async Task Get_State_Returns_Success_StatusCode()
        {
            // Arrange
            var client = factory.WithTestServices(services=> {
                // Services...
            }).CreateClient();

            // Act

            var response = await client.GetAsync(endpoint);

            // Assert

            response.Should().NotBeNull();
            response.EnsureSuccessStatusCode();
        }
    }
}
