using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace APIGateway.Tests
{
    public static class Extensions
    {
        public static WebApplicationFactory<Startup> WithTestServices(this WebApplicationFactory<Startup> factory, Action<IServiceCollection> configuration)
        {
            return factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(configuration);
            });
        }
    }
}
