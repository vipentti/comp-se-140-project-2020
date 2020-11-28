using System.IO;
using System.Text;
using APIGateway.Features.Messages;
using APIGateway.Features.States;
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

            ApplicationState currentState = ApplicationState.Init;

            endpoints.MapGet("/state", async context =>
            {
                await context.Response.WriteAsync(currentState.ToString());
            });

            endpoints.MapPut("/state", async context =>
            {
                context.Request.EnableBuffering();

                var req = context.Request;
                string bodyStr = "";

                // Arguments: Stream, Encoding, detect encoding, buffer size
                // AND, the most important: keep stream opened
                using (StreamReader reader
                        = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                }

                if (ApplicationState.FromName(bodyStr) is ApplicationState nextState) {
                    currentState = nextState;
                }

                await context.Response.WriteAsync(currentState.ToString());
            });
        }
    }
}
