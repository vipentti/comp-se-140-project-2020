using Common;
using Common.Enumerations;
using Common.Messages;
using Common.Options;
using Common.States;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Observer
{
    public class Observer : BackgroundService, IStateChangeListener<ApplicationState.InitState>
    {
        private readonly ILogger<Observer> logger;
        private readonly IRabbitClient client;
        private readonly Settings settings;
        private readonly IFileSystem fileSystem;
        private readonly CommonOptions options;
        private readonly IDateTimeService dateTime;
        private readonly ISharedStateService sharedState;

        public class Settings
        {
            public string OutFilePath { get; set; } = "";
        }

        public Observer(IConfiguration config, IRabbitClient client, IFileSystem fileSystem, ILogger<Observer> logger, IOptions<CommonOptions> options, IDateTimeService dateTime, ISharedStateService sharedState)
        {
            this.logger = logger;
            this.client = client;
            this.fileSystem = fileSystem;

            client.OnMessageReceived += OnMessageReceived;

            settings = config.Get<Settings>();
            this.options = options.Value;
            this.dateTime = dateTime;
            this.sharedState = sharedState;
        }

        private static async Task Main(string[] args)
        {
            await ProgramCommon.Execute(args, services =>
            {
                services.AddHostedService<Observer>();
            });
        }

        public void OnMessageReceived(object? _, Message message)
        {
            logger.LogInformation("Received message {@Message}", message);

            string output = new TopicMessage(dateTime.UtcNow, message.Topic, message.Content).ToString();

            logger.LogInformation("writing to {path}: '{output}'", settings.OutFilePath, output);

            fileSystem.File.AppendAllText(settings.OutFilePath, output + Environment.NewLine);
        }

        public async Task ClearFile()
        {
            logger.LogInformation("Clearing file {path}", settings.OutFilePath);

            await fileSystem.File.WriteAllTextAsync(settings.OutFilePath, "");
        }

        public async Task OnStateChange(ApplicationState.InitState state)
        {
            logger.LogInformation("Received state {State}", state);
            await ClearFile();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await sharedState.SubscribeToChanges(this);
            await ClearFile();

            await client.WaitForRabbitMQ(stoppingToken);

            await client.TryConnect(
                options.ExchangeName,
                options.AllMyTopics,
                stoppingToken);

            logger.LogInformation("Connected");

            await Task.Delay(options.DelayAfterConnect, stoppingToken);

            // keep the service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(options.LoopDelay, stoppingToken);
            }

            logger.LogInformation("Finished");
        }
    }
}
