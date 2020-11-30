using Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Common.RedisSupport;
using Common.States;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Original
{
    public class Original : BackgroundService, IStateChangeListener
    {
        //public static async Task Main(string[] args)
        //{
        //    await ProgramCommon.Execute(args, svc =>
        //    {
        //        svc.AddHostedService<Original>();
        //    });
        //}

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
                .ConfigureWebHostDefaults(builder =>
                {
                    builder
                        .UseSerilog()
                        .UseKestrel()
                        .UseStartup<Startup>();
                });

        public void Reset()
        {
            messageToSend = 1;
        }

        public void Init()
        {
            Pause();
            Reset();
            Resume();
        }

        public void Pause()
        {
            logger.LogInformation("Pausing {Name}", nameof(Original));
            paused = true;
        }

        public void Resume()
        {
            logger.LogInformation("Resuming {Name}", nameof(Original));
            paused = false;
        }

        public async override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting...");
            await base.StartAsync(cancellationToken);
            logger.LogInformation("Start finished");
        }

        public async override Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping...");
            await base.StopAsync(cancellationToken);
            logger.LogInformation("Stopped");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await sharedState.SubscribeToChanges(this);

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            await rabbitClient.WaitForRabbitMQ(stoppingToken);

            await rabbitClient.TryConnect(options.ExchangeName, options.OriginalTopic, stoppingToken);

            logger.LogInformation("Connected");

            await Task.Delay(options.DelayAfterConnect, stoppingToken);

            //{
            //    var sub = await redisClient.GetSubscriber();
            //    var channel = await sub.SubscribeAsync("state-change");
            //    channel.OnMessage(OnStateChange);
            //}

            while (!stoppingToken.IsCancellationRequested)
            {
                if (paused)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
                    continue;
                }
                rabbitClient.SendMessage($"MSG_{messageToSend}");
                ++messageToSend;
                await Task.Delay(options.DelayBetweenMessages, stoppingToken);
            }

            logger.LogInformation("Finished sending messages after {TotalSeconds} seconds", stopWatch.Elapsed.TotalSeconds);
        }

        public async Task OnStateChange(ApplicationState state)
        {
            logger.LogInformation("Received state change {@State}", state);
            await Task.Delay(0);

            var handlers = new Dictionary<ApplicationState, Action>()
            {
                { ApplicationState.Paused, Pause },
                { ApplicationState.Running, Resume },
                { ApplicationState.Init,  Init },
            };

            if (handlers.ContainsKey(state))
            {
                handlers[state]();
            }
        }

        private readonly ILogger<Original> logger;
        private readonly IRabbitClient rabbitClient;
        private readonly ISharedStateService sharedState;
        private readonly CommonOptions options;
        private int messageToSend = 1;
        private bool paused = false;

        public Original(IRabbitClient rabbitClient, ISharedStateService sharedState, ILogger<Original> logger, IOptions<CommonOptions> options)
        {
            this.rabbitClient = rabbitClient;
            this.sharedState = sharedState;
            this.logger = logger;
            this.options = options.Value;
        }
    }
}
