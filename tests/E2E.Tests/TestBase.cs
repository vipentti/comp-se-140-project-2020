
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        // optionally configure extra services in derived classes
        protected virtual void ConfigureServices(IServiceCollection services)
        {
        }

        protected async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request)
        {
            var factory = ServiceProvider.GetService<IHttpClientFactory>();

            var client = factory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);

            return await client.SendAsync(request);
        }
    }
}
