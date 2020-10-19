using Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Intermediate
{
    internal class Intermediate : BackgroundService
    {
        private readonly ILogger<Intermediate> logger;
        private readonly RabbitClient original;
        private readonly RabbitClient intermediate;

        public Intermediate(IServiceProvider provider, ILogger<Intermediate> logger)
        {
            this.original = provider.GetService<RabbitClient>();
            this.intermediate = provider.GetService<RabbitClient>();
            this.logger = logger;

            this.original.OnMessageReceived += OnOriginalMessage;
        }

        public static async Task Main(string[] args)
        {
            await ProgramCommon.Execute(args, svc =>
            {
                svc.AddHostedService<Intermediate>();
            });
        }

        public async void OnOriginalMessage(object? _, Message message)
        {
            logger.LogInformation("Received message {@Message} from original", message);

            await Task.Delay(Constants.IntermediateDelay);

            intermediate.SendMessage($"Got {message.Content}");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ProgramCommon.WaitForRabbitMQ(stoppingToken);

            await original.TryConnect(Constants.RabbitUri, Constants.ExchangeName, Constants.OriginalTopic, stoppingToken);
            await intermediate.TryConnect(Constants.RabbitUri, Constants.ExchangeName, Constants.IntermediateTopic, stoppingToken);

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
