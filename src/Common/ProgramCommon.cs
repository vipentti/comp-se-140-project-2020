using Common.Enumerations;
using Common.Options;
using Common.RedisSupport;
using Common.States;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO.Abstractions;
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

        public static async Task WaitForExponentialDelay(this Random random, int exponent, CancellationToken stoppingToken)
        {
            int randomDelay = random.Next(100, 670);
            int secondsToWait = (int)Math.Pow(2, exponent);
            int delay = randomDelay + secondsToWait * 1000;

            Log.Information("Waiting for {Delay} milliseconds", delay);

            await Task.Delay(delay, stoppingToken);
        }

        /// <summary>
        /// Try to connect using TCP to the specified <paramref name="host"/> and <paramref
        /// name="port"/>. If connection fails, it is re-attempted after a delay which increases
        /// explonentially until <paramref name="maxAttempts"/> is reached. After which an <seealso
        /// cref="InvalidOperationException"/> is thrown
        /// </summary>
        public static async Task WaitForTcpConnection(string host, int port, int maxAttempts = 6, CancellationToken stoppingToken = new CancellationToken())
        {
            using var client = new TcpClient();

            Log.Information("Waiting for Tcp connection to {Host}:{Port}", host, port);

            int attempt = 0;
            var random = new Random();

            while (true)
            {
                try
                {
                    ++attempt;
                    var source = new CancellationTokenSource(5000);
                    using var linked = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, source.Token);
                    await client.ConnectAsync(host, port, linked.Token);
                    break;
                }
                catch
                {
                    await random.WaitForExponentialDelay(attempt - 1, stoppingToken);

                    // We have waited long enough
                    if (attempt > maxAttempts)
                    {
                        Log.Fatal("Unable to connect to {Host}:{Port}", host, port);
                        throw new InvalidOperationException($"Unable to connect to {host}:{port}");
                    }
                }
            }

            Log.Information("Tcp connection to {Host}:{Port} was successful", host, port);
        }

        public static void ConfigureSerilog()
        {
            var configBuilder = new ConfigurationBuilder();

            ConfigureApplication(configBuilder);

            var appSettings = configBuilder.Build();

            var loggerConfig = new LoggerConfiguration()
                .ReadFrom
                .Configuration(appSettings)
                .Destructure
                .With<EnumerationDestructurePolicy>();

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

        public static void AddStateServices(this IServiceCollection services)
        {
            services.AddSingleton<RedisStateService>();
            services.AddSingleton<IReadonlyStateService>(cont => cont.GetRequiredService<RedisStateService>());
            services.AddSingleton<IStateService>(cont => cont.GetRequiredService<RedisStateService>());
            services.AddSingleton<IRedisClient, RedisClient>();
            services.AddSingleton<ISharedStateService, SharedStateService>();
            services.AddHostedService<ShutdownListener>();
        }

        public static void ConfigureCommonServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<RabbitClient>();
            services.AddTransient<IRabbitClient, RabbitClient>();
            services.AddTransient<IDateTimeService, DateTimeService>();

            // Register FileSystem
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<ISessionService, DefaultSessionService>();

            //services.AddSingleton<RedisStateService>();
            //services.AddSingleton<IReadonlyStateService>(cont => cont.GetRequiredService<RedisStateService>());
            //services.AddSingleton<IStateService>(cont => cont.GetRequiredService<RedisStateService>());
            //services.AddSingleton<IRedisClient, RedisClient>();
            //services.AddSingleton<ISharedStateService, SharedStateService>();

            services.Configure<RabbitMQOptions>(configuration.GetSection("RabbitMQ"));
            services.Configure<CommonOptions>(configuration);

            //services.AddHostedService<ShutdownListener>();
            services.AddStateServices();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, Action<IServiceCollection> configureServices) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration(ConfigureApplication)
                .ConfigureServices((hostContext, services) =>
                {
                    configureServices(services);
                    services.ConfigureCommonServices(hostContext.Configuration);
                    /*
                    services.AddTransient<RabbitClient>();
                    services.AddTransient<IRabbitClient, RabbitClient>();
                    services.AddTransient<IDateTimeService, DateTimeService>();

                    // Register FileSystem
                    services.AddSingleton<IFileSystem, FileSystem>();

                    configureServices(services);

                    var config = hostContext.Configuration;

                    services.Configure<RabbitMQOptions>(config.GetSection("RabbitMQ"));
                    services.Configure<CommonOptions>(config);
                    */
                });
    }
}
