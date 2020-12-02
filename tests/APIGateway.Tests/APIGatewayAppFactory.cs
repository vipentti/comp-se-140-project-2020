using APIGateway.Clients;
using APIGateway.Features.States;
using APIGateway.Features.Statistics;
using APIGateway.Tests.Features.Messages;
using Common.States;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIGateway.Tests
{
    public class TestMonitoringClient : IRabbitMonitoringClient
    {
        public Task<IEnumerable<NodeStatistic>> GetNodeStatistics()
        {
            return Task.FromResult<IEnumerable<NodeStatistic>>(new[]
            {
                new NodeStatistic
                {
                    Name = "rabbit@rabbitmq"
                },
            });
        }
    }

    public class APIGatewayAppFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.ReplaceTransient<IMessageApiService, TestMessageService>();
                services.ReplaceSingleton<IStateService, InMemorySessionStateService>();
                services.ReplaceSingleton<IRabbitMonitoringClient, TestMonitoringClient>();
            });
        }
    }
}
