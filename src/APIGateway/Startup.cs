using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using APIGateway.Features.Messages;
using APIGateway.Features.Original;
using APIGateway.Features.States;
using APIGateway.Utils;
using Common;
using Common.RedisSupport;
using Common.States;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Refit;

namespace APIGateway
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            // Register custom formatters for Enumertion types
            services.AddMvcCore(opts =>
            {
                opts.OutputFormatters.Insert(0, new EnumerationOutputFormatter());
                opts.InputFormatters.Insert(0, new EnumerationInputFormatter());
            });

            var apiOptions = Configuration.Get<APIOptions>();
            services.AddHttpClient<IMessageService, MessageService>(svc =>
            {
                svc.BaseAddress = new System.Uri(apiOptions.HttpServerUrl);
            });

            services.AddRefitClient<IOriginalService>(new RefitSettings(new EnumerationSerializer()))
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new System.Uri(Configuration.Get<APIOptions>().OriginalServerUrl);
                });

            services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddSingleton<ISessionService, SessionService>();

            services.AddStateServices();
            services.AddHostedService<InitListener>();
            //services.AddSingleton<IRedisClient, RedisClient>();

            //// services.AddSingleton<IStateService, SessionStateService>();
            //services.AddSingleton<IStateService, RedisStateService>();
            //services.AddSingleton<IRunLogService, InMemoryRunLogService>();

            services.Configure<APIOptions>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment _)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
