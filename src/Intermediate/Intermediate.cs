using Common;
using Common.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly CommonOptions options;

        public Intermediate(IRabbitClient original, IRabbitClient intermediate, ILogger<Intermediate> logger, IOptions<CommonOptions> options)
        {
            this.original = original;
            this.intermediate = intermediate;

            if (ReferenceEquals(this.intermediate, this.original))
            {
                throw new ArgumentException($"{nameof(original)} and {nameof(intermediate)} cannot refer to the same object");
            }

            this.logger = logger;
            this.original.OnMessageReceived += OnOriginalMessage;
            this.options = options.Value;
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

            await Task.Delay(options.IntermediateDelay);

            intermediate.SendMessage($"Got {message.Content}");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await original.WaitForRabbitMQ(stoppingToken);

            await original.TryConnect(options.ExchangeName, options.OriginalTopic, stoppingToken);
            await intermediate.TryConnect(options.ExchangeName, options.IntermediateTopic, stoppingToken);

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
