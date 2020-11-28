using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        public interface IRoute : IRequestHandler
        {
            HttpMethod Method { get; init; }

            string Pattern { get; init; }

            Type HandlerType { get; }
        }

        public class Route<THandler> : IRoute
            where THandler : IRequestHandler
        {
            public HttpMethod Method { get; init; } = HttpMethod.Get;

            public string Pattern { get; init; } = "/";

            public Type HandlerType { get; } = typeof(THandler);

            public async Task Handle(HttpContext context)
            {
                var innerHandler = context.RequestServices.GetRequiredService<THandler>();
                await innerHandler.Handle(context);
            }
        }

        private static readonly IEnumerable<IRoute> Routes = new IRoute[] {
            new Route<MessagesHandler>()
            {
                Method = HttpMethod.Get,
                Pattern = "/messages"
            },
            new Route<StateGetHandler>()
            {
                Method = HttpMethod.Get,
                Pattern = "/state"
            },
            new Route<StatePutHandler>()
            {
                Method = HttpMethod.Put,
                Pattern = "/state"
            },
        };

        public delegate IEndpointConventionBuilder EndpointConfigureDelegate(IEndpointRouteBuilder endpoint, string patten, RequestDelegate requestDelegate);

        public static void UseApplicationRoutes(this IEndpointRouteBuilder endpoints, IHostEnvironment _)
        {
            foreach (var route in Routes)
            {
                EndpointConfigureDelegate configureRoute =
                    route.Method switch {
                        HttpMethod it when it == HttpMethod.Put => EndpointRouteBuilderExtensions.MapPut,
                        _ => EndpointRouteBuilderExtensions.MapGet
                    };

                configureRoute(endpoints, route.Pattern, route.Handle);
            }
        }
    }
}
