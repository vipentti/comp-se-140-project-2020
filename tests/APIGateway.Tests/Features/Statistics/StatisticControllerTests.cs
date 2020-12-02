﻿using APIGateway.Features.Statistics;
using FluentAssertions;
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
