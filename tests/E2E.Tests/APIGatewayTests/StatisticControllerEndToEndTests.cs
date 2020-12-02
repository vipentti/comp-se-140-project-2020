using APIGateway.Tests.Features.Statistics;
using Common.Options;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace E2E.Tests.APIGatewayTests
{
    public class StatisticControllerEndToEndTests : StatisticControllerTestBase
    {
        private readonly APIOptions options;

        private HttpClient _client;

        protected override HttpClient ApiClient
        {
            get
            {
                if (_client is null)
                {
                    _client = new HttpClient
                    {
                        BaseAddress = new Uri(options.ApiGatewayUrl)
                    };
                    _client.DefaultRequestHeaders.Add("X-Session-Id", System.Guid.NewGuid().ToString());
                }

                return _client;
            }
        }

        public StatisticControllerEndToEndTests() : base()
        {
            var configBuilder = new ConfigurationBuilder();

            Common.ProgramCommon.ConfigureApplication(configBuilder);

            var config = configBuilder.Build();

            options = config.Get<APIOptions>();
        }

        [E2EFact]
        public override Task Get_NodeStatistics_Responds_Ok() => base.Get_NodeStatistics_Responds_Ok();

        [E2EFact]
        public override Task Get_NodeStatistics_Returns_NodeStatisticData() => base.Get_NodeStatistics_Returns_NodeStatisticData();

        [E2EFact]
        public override Task Get_QueueStatistics_Responds_Ok() => base.Get_QueueStatistics_Responds_Ok();

        [E2EFact]
        public override Task Get_QueueStatistics_Returns_Data() => base.Get_QueueStatistics_Returns_Data();
    }
}
