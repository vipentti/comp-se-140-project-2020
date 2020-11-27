using Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace Original
{
    public class Original : BackgroundService
    {
        public static async Task Main(string[] args)
        {
            await ProgramCommon.Execute(args, svc =>
            {
                svc.AddHostedService<Original>();
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            await rabbitClient.WaitForRabbitMQ(stoppingToken);

            await rabbitClient.TryConnect(options.ExchangeName, options.OriginalTopic, stoppingToken);

            logger.LogInformation("Connected");

            await Task.Delay(options.DelayAfterConnect, stoppingToken);

            int messageToSend = 1;

            while (!stoppingToken.IsCancellationRequested
                   && messageToSend <= options.MaximumNumberOfMessagesToSend)
            {
                rabbitClient.SendMessage($"MSG_{messageToSend}");
                ++messageToSend;
                await Task.Delay(options.DelayBetweenMessages, stoppingToken);
            }

            logger.LogInformation("Finished sending messages after {TotalSeconds} seconds", stopWatch.Elapsed.TotalSeconds);
        }

        private readonly ILogger<Original> logger;
        private readonly IRabbitClient rabbitClient;
        private readonly CommonOptions options;

        public Original(IRabbitClient rabbitClient, ILogger<Original> logger, IOptions<CommonOptions> options)
        {
            this.rabbitClient = rabbitClient;
            this.logger = logger;
            this.options = options.Value;
        }
    }
}
