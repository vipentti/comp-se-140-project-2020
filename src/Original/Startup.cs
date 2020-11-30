using Common;
using Common.Enumerations;
using Common.RedisSupport;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Original
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

            services.AddMvcCore(opts =>
            {
                opts.OutputFormatters.Insert(0, new EnumerationOutputFormatter());
                opts.InputFormatters.Insert(0, new EnumerationInputFormatter());
            });

            // Common.ProgramCommon.ConfigureCommonServices(services, Configuration);

            services.ConfigureCommonServices(Configuration);

            //services.AddSingleton<IRedisClient, RedisClient>();
            services.AddSingleton<Original>();
            services.AddHostedService(it => it.GetRequiredService<Original>());
            //var apiOptions = Configuration.Get<APIOptions>();
            //services.AddHttpClient<IMessageService, MessageService>(svc =>
            //{
            //    svc.BaseAddress = new System.Uri(apiOptions.HttpServerUrl);
            //});

            //services.AddTransient<IDateTimeService, DateTimeService>();

            //services.AddSingleton<IStateService, SessionStateService>();
            //services.AddSingleton<IRunLogService, InMemoryRunLogService>();

            //services.Configure<APIOptions>(Configuration);
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
