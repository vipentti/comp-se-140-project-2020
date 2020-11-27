using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Common;
using System.Threading;
using Microsoft.Extensions.Logging;

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

            await rabbitClient.TryConnect(Constants.ExchangeName, Constants.OriginalTopic, stoppingToken);

            logger.LogInformation("Connected");

            await Task.Delay(Constants.DelayAfterConnect, stoppingToken);

            int messageToSend = 1;

            while (!stoppingToken.IsCancellationRequested
                   && messageToSend <= Constants.MaximumNumberOfMessagesToSend)
            {
                rabbitClient.SendMessage($"MSG_{messageToSend}");
                ++messageToSend;
                await Task.Delay(Constants.DelayBetweenMessages, stoppingToken);
            }

            logger.LogInformation("Finished sending messages after {TotalSeconds} seconds", stopWatch.Elapsed.TotalSeconds);
        }

        private readonly ILogger<Original> logger;
        private readonly IRabbitClient rabbitClient;

        public Original(IRabbitClient rabbit, ILogger<Original> logger)
        {
            this.rabbitClient = rabbit;
            this.logger = logger;
        }
    }
}
