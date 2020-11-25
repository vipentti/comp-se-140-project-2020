using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;

namespace E2E.Tests
{
    public class HttpServerTests
    {
        public class Settings
        {
            public string HttpServerUrl { get; set; } = "";
        }

        private readonly IConfigurationRoot configuration;

        private readonly IServiceScopeFactory scopeFactory;

        private readonly Settings settings;

        public HttpServerTests()
        {
            var configBuilder = new ConfigurationBuilder();

            Common.ProgramCommon.ConfigureApplication(configBuilder);

            configuration = configBuilder.Build();

            settings = configuration.Get<Settings>();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddHttpClient();

            scopeFactory = serviceCollection.BuildServiceProvider().GetService<IServiceScopeFactory>();
        }

        [Fact]
        public async Task Test_HttpServer_Returns_OK_Response()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "");

            var response = await SendRequest(request);
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Test_HttpServer_Returns_Messages()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "");

            var response = await SendRequest(request);
            response.Should().NotBeNull();
            response.IsSuccessStatusCode.Should().BeTrue();

            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();

            // Ensure lines are normalized
            var lines = content.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);

            lines.Should().HaveCountGreaterOrEqualTo(2);

            // We don't care about the timestamps in this case
            lines[0].Should().Match("* Topic my.o: MSG_1");
            lines[1].Should().Match("* Topic my.i: Got MSG_1");
        }

        private async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request)
        {
            using var scope = scopeFactory.CreateScope();
            var services = scope.ServiceProvider;

            var factory = services.GetService<IHttpClientFactory>();

            var client = factory.CreateClient();
            client.BaseAddress = new Uri(settings.HttpServerUrl);
            client.Timeout = TimeSpan.FromSeconds(5);

            return await client.SendAsync(request);
        }
    }
}
