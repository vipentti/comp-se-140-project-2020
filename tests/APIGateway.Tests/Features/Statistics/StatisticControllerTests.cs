using FluentAssertions;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace APIGateway.Tests.Features.Statistics
{
    public class StatisticControllerTests : IClassFixture<APIGatewayAppFactory>
    {
        private const string NodeStatisticEndpoint = "/node-statistic";
        private readonly APIGatewayAppFactory factory;

        private HttpClient _client;

        private HttpClient HttpClient
        {
            get
            {
                if (_client is null)
                {
                    _client = factory.WithTestServices(services =>
                    {
                        services.SetupMockServices();
                    }).CreateClient();
                }

                return _client;
            }
        }

        public StatisticControllerTests(APIGatewayAppFactory factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task Get_NodeStatistics_Responds_Ok()
        {
            // Act
            var response = await HttpClient.GetAsync(NodeStatisticEndpoint);

            // Assert

            response.Should().NotBeNull();
            response.EnsureSuccessStatusCode();
        }
    }
}
