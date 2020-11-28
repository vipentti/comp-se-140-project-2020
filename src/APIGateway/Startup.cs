using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using APIGateway.Features.Messages;
using APIGateway.Features.States;
using APIGateway.Utils;
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
            var assembly = Assembly.GetAssembly(typeof(Startup));

            if (assembly != null)
            {
                // Mark enumerations using the converter attribute.
                //
                // This allows ApiDescription to support these like regular enums with string
                // conversion. Without this, enumerations are treated as regular classes when used
                // as parameters to controller actions.
                foreach (var enumType in Enumeration.GetEnumerationTypes(assembly))
                {
                    TypeDescriptor.AddAttributes(enumType, new TypeConverterAttribute(typeof(EnumerationTypeConverter)));
                }
            }

            services.AddMvcCore(opts =>
            {
                // Register modelbinder for enumerations
                //opts.ModelBinderProviders.Insert(0, new EnumerationModelBinderProvider());
                opts.OutputFormatters.Insert(0, new EnumerationOutputFormatter());
                opts.InputFormatters.Insert(0, new EnumerationInputFormatter());
            })
                .AddJsonOptions(opts =>
                {
                    // Convert all enums in the APIs into strings and from strings
                    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    opts.JsonSerializerOptions.Converters.Add(new EnumerationJsonConverterFactory());

                    opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    opts.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                });

            var apiOptions = Configuration.Get<APIOptions>();
            services.AddHttpClient<IMessageService, MessageService>(svc =>
            {
                svc.BaseAddress = new System.Uri(apiOptions.HttpServerUrl);
            });

            //var requestHandlers = Assembly.GetAssembly(typeof(Startup))
            //    ?.GetTypes()
            //    ?.Where(type => type.IsClass && !type.IsAbstract && type.IsAssignableTo(typeof(IRequestHandler)))
            //    ?.ToList() ?? new System.Collections.Generic.List<System.Type>();

            //services.AddTransient<MessagesHandler>();
            //services.AddTransient<StateGetHandler>();
            //services.AddTransient<StatePutHandler>();
            //services.AddTransient<RunLogGetHandler>();
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
                endpoints.MapControllers();
                //endpoints.UseApplicationRoutes(env);
            });
        }
    }
}
