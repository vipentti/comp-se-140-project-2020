using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public static class ProgramCommon
    {
        public static async Task Execute(string[] args, Action<IServiceCollection> configure)
        {
            ConfigureSerilog();

            try
            {
                var host = CreateHostBuilder(args, configure).Build();
                Log.Information("Running application");
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal("Uncaught exception: {Exception}", ex);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        ///<summary>
        /// Wait for RabbitMQ to start up
        ///</summary>
        public static async Task WaitForRabbitMQ(CancellationToken stoppingToken = new CancellationToken())
        {
            using var client = new TcpClient();

            Log.Information("Waiting for RabbitMQ");

            while (true)
            {
                try
                {
                    var source = new CancellationTokenSource(1000);
                    using var linked = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, source.Token);
                    await client.ConnectAsync(Constants.RabbitHost, Constants.RabbitPort, linked.Token);
                    break;
                }
                catch
                {
                }
            }

            Log.Information("RabbitMQ should be up!");
        }

        public static void ConfigureSerilog()
        {
            var configBuilder = new ConfigurationBuilder();

            ConfigureApplication(configBuilder);

            var appSettings = configBuilder.Build();

            var loggerConfig = new LoggerConfiguration()
                .ReadFrom
                .Configuration(appSettings);

            Log.Logger = loggerConfig.CreateLogger();
        }

        public static void ConfigureApplication(IConfigurationBuilder configuration)
        {
            configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true, reloadOnChange: false)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, Action<IServiceCollection> configureServices) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration(ConfigureApplication)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<RabbitClient>();
                    configureServices(services);
                });
    }
}
