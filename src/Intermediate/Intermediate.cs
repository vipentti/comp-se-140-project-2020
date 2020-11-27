using Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Intermediate
{
    public class Intermediate : BackgroundService
    {
        private readonly ILogger<Intermediate> logger;
        private readonly IRabbitClient original;
        private readonly IRabbitClient intermediate;

        public Intermediate(IRabbitClient original, IRabbitClient intermediate, ILogger<Intermediate> logger)
        {
            this.original = original;
            this.intermediate = intermediate;

            if (ReferenceEquals(this.intermediate, this.original)) {
                throw new ArgumentException($"{nameof(original)} and {nameof(intermediate)} cannot refer to the same object");
            }

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
            await original.WaitForRabbitMQ(stoppingToken);

            await original.TryConnect(Constants.ExchangeName, Constants.OriginalTopic, stoppingToken);
            await intermediate.TryConnect(Constants.ExchangeName, Constants.IntermediateTopic, stoppingToken);

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
