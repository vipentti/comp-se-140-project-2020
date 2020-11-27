using APIGateway.Features.Messages;
using APIGateway.Tests.Features.Messages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.AspNetCore.TestHost;

namespace APIGateway.Tests
{
    public class APIGatewayAppFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // base.ConfigureWebHost(builder);
            builder.ConfigureServices(services => {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(IMessageService));

                services.Remove(descriptor);

                services.AddTransient<IMessageService, TestMessageService>();
            });
        }
    }
}
