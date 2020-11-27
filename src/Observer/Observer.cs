using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Abstractions;

namespace Observer
{
    public class Observer : BackgroundService
    {
        private readonly ILogger<Observer> logger;
        private readonly IRabbitClient client;
        private readonly Settings settings;
        private readonly IFileSystem fileSystem;

        public class Settings
        {
            public string OutFilePath { get; set; } = "";
        }

        public Observer(IConfiguration config, IRabbitClient client, IFileSystem fileSystem, ILogger<Observer> logger)
        {
            this.logger = logger;
            this.client = client;
            this.fileSystem = fileSystem;

            client.OnMessageReceived += OnMessageReceived;

            settings = config.Get<Settings>();
        }

        static async Task Main(string[] args)
        {
            await ProgramCommon.Execute(args, services =>
            {
                services.AddHostedService<Observer>();
            });
        }

        public void OnMessageReceived(object? _, Message message)
        {
            logger.LogInformation("Received message {@Message}", message);

            string output = $"{DateTime.UtcNow.ToISO8601()} Topic {message.Topic}: {message.Content}";

            logger.LogInformation("writing to {path}: '{output}'", settings.OutFilePath, output);

            fileSystem.File.AppendAllText(settings.OutFilePath, output + Environment.NewLine);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Clearing file {path}", settings.OutFilePath);

            System.IO.File.WriteAllText(settings.OutFilePath, "");

            await client.WaitForRabbitMQ(stoppingToken);

            await client.TryConnect(
                Constants.ExchangeName,
                Constants.AllMyTopics,
                stoppingToken);

            logger.LogInformation("Connected");

            await Task.Delay(Constants.DelayAfterConnect, stoppingToken);

            // keep the service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(Constants.LoopDelay, stoppingToken);
            }

            logger.LogInformation("Finished");
        }
    }
}
