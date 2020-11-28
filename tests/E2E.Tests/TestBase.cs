using APIGateway;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace E2E.Tests
{
    public abstract class TestBase
    {
        protected IConfigurationRoot Configuration { get; }

        protected IServiceProvider ServiceProvider { get; }

        protected TestBase()
        {
            var configBuilder = new ConfigurationBuilder();

            Common.ProgramCommon.ConfigureApplication(configBuilder);

            Configuration = configBuilder.Build();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddHttpClient();

            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private readonly Guid sessionId = Guid.NewGuid();

        // optionally configure extra services in derived classes
        protected virtual void ConfigureServices(IServiceCollection services)
        {
        }

        protected async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request)
        {
            var factory = ServiceProvider.GetService<IHttpClientFactory>();

            var client = factory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            client.BaseAddress = new Uri(Configuration.Get<APIOptions>().ApiGatewayUrl);
            client.DefaultRequestHeaders.Add("X-Session-Id", sessionId.ToString());

            return await client.SendAsync(request);
        }

        protected async Task<HttpResponseMessage> GetRequest(string endpoint)
        {
            return await SendRequest(new HttpRequestMessage(method: HttpMethod.Get, requestUri: endpoint));
        }

        protected async Task<HttpResponseMessage> PutRequest(string endpoint, HttpContent content)
        {
            return await SendRequest(new HttpRequestMessage(method: HttpMethod.Put, requestUri: endpoint)
            {
                Content = content
            });
        }

        protected Task<HttpResponseMessage> PutStringRequest<T>(string endpoint, T content)
        {
            return PutRequest(endpoint, new StringContent(content.ToString()));
        }
    }
}
