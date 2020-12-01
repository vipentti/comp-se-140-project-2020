using Common;
using Common.Enumerations;
using Common.States;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
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

                using (var scope = host.Services.CreateScope())
                {
                    var stateService = scope.ServiceProvider.GetRequiredService<IStateService>();

                    await stateService.ClearRunLogEntries();
                    await stateService.SetCurrentState(ApplicationState.Init);
                    await stateService.SetCurrentState(ApplicationState.Running);
                }

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
                        .UseStartup<Startup>();
                });
    }
}
