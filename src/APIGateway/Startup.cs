using APIGateway.Clients;
using APIGateway.Features.Messages;
using APIGateway.Features.Original;
using APIGateway.Features.States;
using APIGateway.Utils;
using Common;
using Common.Messages;
using Common.States;
using Microsoft.AspNetCore.Builder;
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
                opts.OutputFormatters.Insert(0, new Common.Enumerations.EnumerationOutputFormatter());
                opts.OutputFormatters.Insert(0, new Common.Formatters.PlainTextOutputFormatter<RunLogEntry>());
                opts.OutputFormatters.Insert(0, new Common.Formatters.PlainTextOutputFormatter<TopicMessage>());

                opts.InputFormatters.Insert(0, new Common.Enumerations.EnumerationInputFormatter());
                opts.InputFormatters.Insert(0, new Common.Formatters.PlainTextInputFormatter<RunLogEntry>(
                    RunLogEntry.FromString,
                    its => its.RunLogEntriesFromString()
                ));

                opts.InputFormatters.Insert(0, new Common.Formatters.PlainTextInputFormatter<TopicMessage>(
                    TopicMessage.FromString,
                    its => its.TopicMessagesFromString()
                ));

                //opts.InputFormatters.Insert(0, new Common.Enumerations.EnumerationInputFormatter());
            });

            var apiOptions = Configuration.Get<APIOptions>();
            services.AddHttpClient<IMessageService, MessageService>(svc =>
            {
                svc.BaseAddress = new System.Uri(apiOptions.HttpServerUrl);
            });

            services.AddRefitClient<IMessageApiService>(new RefitSettings(new ApiContentSerializer()))
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new System.Uri(apiOptions.HttpServerUrl);
                });

            services.AddRefitClient<IOriginalService>(new RefitSettings(new ApiContentSerializer()))
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
