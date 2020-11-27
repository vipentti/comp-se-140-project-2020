using Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading.Tasks;

namespace APIGateway
{
    public static class Program
    {
        public static async Task Main(string[] args)
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
                .ConfigureWebHostDefaults(builder => {
                    builder
                        .UseSerilog()
                        .UseKestrel()
                        .UseStartup<Startup>();
                });
    }
}
