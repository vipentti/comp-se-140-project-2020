using Common;
using Microsoft.AspNetCore;
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
                var host = CreateWebHostBuilder(args).Build();

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal("Uncaught exception: {Message} {Exception}", ex.Message, ex);
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseKestrel()
                .ConfigureAppConfiguration(ProgramCommon.ConfigureApplication)
                .UseStartup<Startup>();
    }
}
