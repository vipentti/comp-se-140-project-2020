using APIGateway;
using APIGateway.Features.States;
using APIGateway.Tests.Features.States;
using Common;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace E2E.Tests.APIGatewayTests
{
    public class RunLogEndToEndTests : RunLogTestBase
    {
        private readonly APIOptions options;

        protected IConfigurationRoot Configuration { get; }

        protected IServiceProvider ServiceProvider { get; }

        public RunLogEndToEndTests()
        {
            var configBuilder = new ConfigurationBuilder();

            Common.ProgramCommon.ConfigureApplication(configBuilder);

            Configuration = configBuilder.Build();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddHttpClient();

            // ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            options = Configuration.Get<APIOptions>();
        }

        private HttpClient _client;

        protected override HttpClient client
        {
            get
            {
                if (_client is not null)
                {
                    return _client;
                }

                _client = ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();
                _client.BaseAddress = new Uri(options.ApiGatewayUrl);
                _client.DefaultRequestHeaders.Add("X-Session-Id", System.Guid.NewGuid().ToString());
                return _client;
            }
        }

        protected override string endpoint => "/run-log";

        protected override IDateTimeService dateTime => new DateTimeService();

        [E2EFact]
        public override Task Get_RunLog_Returns_Success_StatusCode() => base.Get_RunLog_Returns_Success_StatusCode();

        [E2EFact]
        public override Task Get_RunLog_Returns_Log() => base.Get_RunLog_Returns_Log();

        [E2EFact]
        public override Task Put_ReinitLog_Initializes_Log_Entries_And_Returns_State_Changes() => base.Put_ReinitLog_Initializes_Log_Entries_And_Returns_State_Changes();
    }

    public class RunLogTests : TestBase
    {
        private string Endpoint => $"/run-log";

        public RunLogTests() : base()
        {
            //options = Configuration.Get<APIOptions>();
        }

        [E2EFact]
        public async Task Get_RunLog_Returns_Ok()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, Endpoint);

            using HttpResponseMessage response = await SendRequest(request);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [E2EFact]
        public async Task Get_RunLog_Returns_Entries()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, Endpoint);

            using HttpResponseMessage response = await SendRequest(request);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();

            List<RunLogEntry> entries = content
                .Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(RunLogEntry.FromString)
                .ToList();

            entries.Should().HaveCountGreaterOrEqualTo(1);
        }

        [E2EFact]
        public async Task Put_State_Updates_RunLog()
        {
            // Get initial entries
            var originalEntries = await GetLogEntries();

            var nextState = ApplicationState.Paused;

            var response = await PutRequest("/state", new StringContent(nextState.ToString()));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            content.Should().Be(nextState.ToString());

            var newEntries = await GetLogEntries();
        }

        private async Task<List<RunLogEntry>> GetLogEntries()
        {
            var response = await GetRequest(Endpoint);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNull();

            return ReadEntriesFrom(content);
        }

        public static List<RunLogEntry> ReadEntriesFrom(string content)
        {
            List<RunLogEntry> entries = content
                .Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(RunLogEntry.FromString)
                .ToList();

            return entries;
        }
    }
}
