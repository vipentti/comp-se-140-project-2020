using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Common;

namespace E2E.Tests
{
    /// <summary>
    /// Provides end-to-end testing for the HttpServer. This requires that the HttpServer is running
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

        [E2EFact]
        public async Task Test_HttpServer_Returns_OK_Response()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "");

            var response = await SendRequest(request);
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [E2EFact]
        public async Task Test_HttpServer_Returns_Messages()
        {
            string[] lines = await GetMessages(2);

            lines.Should().HaveCountGreaterOrEqualTo(2);

            // We don't care about the timestamps in this case
            lines[0].Should().Match("* Topic my.o: MSG_1");
            lines[1].Should().Match("* Topic my.i: Got MSG_1");
        }

        [E2EFact]
        public async Task Test_HttpServer_Messages_Contain_Correctly_Formatted_Timestamp()
        {
            // Arrange

            // Act
            string[] messages = await GetMessages(acceptableNumberOfMessages: 1);

            // Assert
            messages.Should().HaveCountGreaterOrEqualTo(1);

            // Messages should be in the following format 2020-11-26T11:30:45.860Z Topic my.o: MSG_1
            string[] parts = messages[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);

            parts.Should().HaveCountGreaterOrEqualTo(1);

            // Datetime should be convertible
            DateTime? messageDate = parts[0].FromISO8601();

            messageDate.Should().NotBeNull();
        }

        private async Task<string[]> GetMessages(int acceptableNumberOfMessages)
        {
            async Task<string> makeRequest()
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, "");

                var response = await SendRequest(request);
                response.Should().NotBeNull();
                response.IsSuccessStatusCode.Should().BeTrue();

                return await response.Content.ReadAsStringAsync();
            }

            const int MAX_ATTEMPTS = 10;
            int attempt = 0;

            var random = new Random();

            string content;

            var defaultOptions = new CommonOptions();

            // If the server returns OK but empty content, it may simply mean that the messages have
            // not been sent yet, so we want to give some time for the messages to be sent before
            // giving up
            do
            {
                ++attempt;

                content = await makeRequest();

                // If we have some content but not the full data try once more to get the data after
                // a delay This should reduce the likelyhood that we read the content just before
                // the next message was added
                if (!string.IsNullOrEmpty(content))
                {
                    var messages = content.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    if (messages.Length >= acceptableNumberOfMessages)
                    {
                        break;
                    }
                }

                int randomDelay = random.Next(100, 300);
                await Task.Delay(TimeSpan.FromMilliseconds(defaultOptions.DelayBetweenMessages + randomDelay));
            } while (attempt < MAX_ATTEMPTS);

            content.Should().NotBeNullOrEmpty(because: $"Failed after {attempt} attempt(s).");

            return content.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);
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
