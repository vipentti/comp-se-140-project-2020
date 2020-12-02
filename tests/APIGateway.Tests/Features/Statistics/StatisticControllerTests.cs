using APIGateway.Features.Statistics;
using FluentAssertions;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace APIGateway.Tests.Features.Statistics
{
    /// <summary>
    /// Base class for statistic controller tests
    /// </summary>
    public abstract class StatisticControllerTestBase
    {
        protected const string NodeStatisticEndpoint = "/node-statistic";
        protected const string QueueStatisticEndpoint = "/queue-statistic";

        protected abstract HttpClient ApiClient { get; }

        [Fact]
        public virtual async Task Get_NodeStatistics_Responds_Ok()
        {
            // Act
            var response = await ApiClient.GetAsync(NodeStatisticEndpoint);

            // Assert

            response.Should().NotBeNull();
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public virtual async Task Get_NodeStatistics_Returns_NodeStatisticData()
        {
            // Act
            var response = await ApiClient.GetAsync(NodeStatisticEndpoint);

            response.Should().NotBeNull();
            response.EnsureSuccessStatusCode();

            NodeStatistic stats = await response.Content.ReadFromJsonAsync<NodeStatistic>();

            stats.Should().NotBeNull();
            stats.Name.Should().Be("rabbit@rabbitmq");
        }

        [Fact]
        public virtual async Task Get_QueueStatistics_Responds_Ok()
        {
            // Act
            var response = await ApiClient.GetAsync(QueueStatisticEndpoint);

            // Assert

            response.Should().NotBeNull();
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public virtual async Task Get_QueueStatistics_Returns_Data()
        {
            async Task<List<FlatQueueStatistic>> makeRequest()
            {
                var response = await ApiClient.GetAsync(QueueStatisticEndpoint);

                response.Should().NotBeNull();
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<List<FlatQueueStatistic>>();
            }

            // Act

            var data = await makeRequest();

            int attempts = 5;

            // The endpoint may return empty set of data before RabbitMQ has been fully started when
            // running e2e tests
            while (data.Count == 0 && attempts >= 0)
            {
                --attempts;
                await Task.Delay(1000);
                data = await makeRequest();
            }

            data.Should().NotBeNullOrEmpty();
        }
    }

    public class StatisticControllerTests : StatisticControllerTestBase, IClassFixture<APIGatewayAppFactory>
    {
        private readonly APIGatewayAppFactory factory;

        private HttpClient _client;

        protected override HttpClient ApiClient
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
    }
}
