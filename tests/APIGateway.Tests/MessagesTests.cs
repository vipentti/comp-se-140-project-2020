using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using APIGateway.Features.Messages;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace APIGateway.Tests
{
    public class MessagesTests : IClassFixture<APIGatewayAppFactory>
    {
        private readonly APIGatewayAppFactory factory;
        private readonly string endpoint = "/messages";

        private readonly Mock<IMessageService> messageServiceMock;

        public MessagesTests(APIGatewayAppFactory factory)
        {
            this.factory = factory;
            this.messageServiceMock = new Mock<IMessageService>(MockBehavior.Strict);
        }

        [Fact]
        public async Task Get_Messages_Calls_GetMessages_From_IMessageService()
        {
            // Arrange
            var client = factory.WithWebHostBuilder(builder => {
                builder.ConfigureTestServices(svc => {
                    svc.AddTransient(_ => messageServiceMock.Object);
                });
            }).CreateClient();

            // Act

            var response = await client.GetAsync(endpoint);

            // Assert

            response.Should().NotBeNull();
            response.EnsureSuccessStatusCode();

            messageServiceMock.Verify(it => it.GetMessages(), Times.Once);
        }
    }
}
