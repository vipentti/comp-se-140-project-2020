using APIGateway.Clients;
using Common.Messages;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace APIGateway.Tests.Features.Messages
{
    public class MessagesTests : IClassFixture<APIGatewayAppFactory>
    {
        private readonly APIGatewayAppFactory factory;
        private readonly string endpoint = "/messages";
        private readonly Mock<IMessageApiService> messageServiceMock;

        public MessagesTests(APIGatewayAppFactory factory)
        {
            this.factory = factory;
            this.messageServiceMock = new Mock<IMessageApiService>(MockBehavior.Strict);
            this.messageServiceMock.Setup(it => it.GetMessages()).ReturnsAsync(Enumerable.Empty<TopicMessage>());
        }

        [Fact]
        public async Task Get_Messages_Calls_GetMessages_From_IMessageService()
        {
            // Arrange
            var client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(svc =>
                {
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
