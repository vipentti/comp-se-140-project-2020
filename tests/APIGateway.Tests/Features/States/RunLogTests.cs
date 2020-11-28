using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using APIGateway.Features.Messages;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Http;
using APIGateway.Features.States;

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
