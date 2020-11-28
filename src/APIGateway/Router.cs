using APIGateway.Features.Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace APIGateway
{
    public static class Router
    {
        public static void UseApplicationRoutes(this IEndpointRouteBuilder endpoints, IHostEnvironment _)
        {
            endpoints.MapGet("/messages", async context =>
            {
                var handler = context.RequestServices.GetRequiredService<MessagesHandler>();
                await handler.Handle(context);
            });

            endpoints.MapGet("/state", async context =>
            {
                await context.Response.WriteAsync("INIT");
            });
        }
    }
}
