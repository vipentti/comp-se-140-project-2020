using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;

namespace E2E.Tests
{
    /// <summary>
    /// Provides end-to-end testing for the HttpServer.
    /// This requires that the HttpServer is running
    /// at the url provided in HttpServerUrl configuration variable.
    /// </summary>
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
            async Task<string> makeRequest() {
                using var request = new HttpRequestMessage(HttpMethod.Get, "");

                var response = await SendRequest(request);
                response.Should().NotBeNull();
                response.IsSuccessStatusCode.Should().BeTrue();

                return await response.Content.ReadAsStringAsync();
            }

            string content = await makeRequest();

            const int MAX_ATTEMPTS = 10;
            int attempt = 0;

            var random = new Random();

            // If the server returns OK but empty content, it may simply mean
            // that the messages  have not been sent yet, so we want to give
            // some time for the messages to be sent before giving up
            while (string.IsNullOrEmpty(content) && attempt < MAX_ATTEMPTS) {
                ++attempt;

                int randomDelay = random.Next(100, 300);
                await Task.Delay(TimeSpan.FromMilliseconds(Common.Constants.DelayBetweenMessages + randomDelay));
                content = await makeRequest();

                // Ensure lines are normalized
                var maybeLines = content.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);
                // If we have some content but not the full data
                // try once more to get the data after a delay
                // This should reduce the likelyhood that we read the content
                // just before the second message was added
                if (!string.IsNullOrEmpty(content) && maybeLines.Length < 2) {
                    randomDelay = random.Next(100, 300);
                    await Task.Delay(TimeSpan.FromMilliseconds(Common.Constants.DelayBetweenMessages + randomDelay));
                    content = await makeRequest();
                    break;
                }
            }

            content.Should().NotBeNullOrEmpty(because: $"Failed after {attempt} attempt(s).");

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
