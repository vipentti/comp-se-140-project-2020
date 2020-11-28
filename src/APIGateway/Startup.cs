
using APIGateway.Features.Messages;
using APIGateway.Features.States;
using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            var apiOptions = Configuration.Get<APIOptions>();
            services.AddHttpClient<IMessageService, MessageService>(svc => {
                svc.BaseAddress = new System.Uri(apiOptions.HttpServerUrl);
            });
            services.AddTransient<MessagesHandler>();
            services.AddTransient<StateGetHandler>();
            services.AddTransient<StatePutHandler>();
            services.AddTransient<IDateTimeService, DateTimeService>();

            services.AddSingleton<IStateService, StateService>();

            services.Configure<APIOptions>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.UseApplicationRoutes(env);
            });
        }
    }
}
