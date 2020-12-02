using APIGateway.Features.Statistics;
using FluentAssertions;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace APIGateway.Tests.Features.Statistics
{
    public class StatisticControllerTests : IClassFixture<APIGatewayAppFactory>
    {
        private const string NodeStatisticEndpoint = "/node-statistic";
        private readonly APIGatewayAppFactory factory;

        private HttpClient _client;

        private HttpClient ApiClient
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
            var response = await ApiClient.GetAsync(NodeStatisticEndpoint);

            // Assert

            response.Should().NotBeNull();
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Get_NodeStatistics_Returns_NodeStatisticData()
        {
            // Act
            var response = await ApiClient.GetAsync(NodeStatisticEndpoint);

            response.Should().NotBeNull();
            response.EnsureSuccessStatusCode();

            var contentString = await response.Content.ReadAsStringAsync();

            NodeStatistic stats = await response.Content.ReadFromJsonAsync<NodeStatistic>();

            stats.Should().NotBeNull();
            stats.Name.Should().Be("rabbit@rabbitmq");
        }
    }
}
