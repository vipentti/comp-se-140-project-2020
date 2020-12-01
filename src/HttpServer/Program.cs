using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading.Tasks;

namespace HttpServer
{
    public class Settings
    {
        public string OutFilePath { get; set; } = "";
    }

    public class Program
    {
        private static async Task Main(string[] args)
        {
            ProgramCommon.ConfigureSerilog();

            try
            {
                var host = CreateHostBuilder(args).Build();

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal("Uncaught exception: {Message} {Exception}", ex.Message, ex);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration(ProgramCommon.ConfigureApplication)
                .ConfigureWebHostDefaults(builder =>
                {
                    builder
                        .UseSerilog()
                        .UseKestrel()
                        .Configure(app =>
                        {
                            app.UseMiddleware<ServerMiddleware>();
                        });
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.ConfigureCommonServices(hostContext.Configuration);
                    services.AddTransient<ServerMiddleware>();
                });
    }
}
