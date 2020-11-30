using APIGateway.Features.Messages;
using APIGateway.Features.States;
using APIGateway.Tests.Features.Messages;
using Common.States;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace APIGateway.Tests
{
    public class APIGatewayAppFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.ReplaceTransient<IMessageService, TestMessageService>();
                services.ReplaceSingleton<IStateService, SessionStateService>();
            });
        }
    }
}
