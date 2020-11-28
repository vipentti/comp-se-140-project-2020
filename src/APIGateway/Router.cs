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
    public interface IRoute : IRequestHandler
    {
        HttpMethod Method { get; }

        string Pattern { get; }
    }

    public static class Router
    {
        //public class Route<THandler> : IRoute
        //    where THandler : IRequestHandler
        //{
        //    public HttpMethod Method { get; init; } = HttpMethod.Get;

        // public string Pattern { get; init; } = "/";

        //    public async Task Handle(HttpContext context)
        //    {
        //        var innerHandler = context.RequestServices.GetRequiredService<THandler>();
        //        await innerHandler.Handle(context);
        //    }
        //}

        private static readonly IEnumerable<IRoute> Routes = new IRoute[] {
            //new Route<MessagesHandler>()
            //{
            //    Method = HttpMethod.Get,
            //    Pattern = "/messages"
            //},
            //new Route<StateGetHandler>()
            //{
            //    Method = HttpMethod.Get,
            //    Pattern = "/state"
            //},
            //new Route<StatePutHandler>()
            //{
            //    Method = HttpMethod.Put,
            //    Pattern = "/state"
            //},
            //new Route<RunLogGetHandler>()
            //{
            //    Method = HttpMethod.Get,
            //    Pattern = "/run-log"
            //},
        };

        public delegate IEndpointConventionBuilder EndpointConfigureDelegate(IEndpointRouteBuilder endpoint, string patten, RequestDelegate requestDelegate);

        public static void UseApplicationRoutes(this IEndpointRouteBuilder endpoints, IHostEnvironment _)
        {
            foreach (var route in Routes)
            {
                EndpointConfigureDelegate configureRoute =
                    route.Method switch
                    {
                        HttpMethod it when it == HttpMethod.Put => EndpointRouteBuilderExtensions.MapPut,
                        HttpMethod it when it == HttpMethod.Get => EndpointRouteBuilderExtensions.MapGet,
                        _ => throw new NotImplementedException($"Routing support for '{route.Method}' is not implemented"),
                    };

                configureRoute(endpoints, route.Pattern, async context =>
                {
                    var routeHandler = context.RequestServices.GetRequiredService(route.GetType());
                });
            }
        }
    }
}
