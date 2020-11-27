using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common
{
    public interface IRabbitClient
    {
        event EventHandler<Message>? OnMessageReceived;

        void SendMessage(string message);

        Task TryConnect(string exchangeName, string routingKey, CancellationToken stoppingToken);

        Task WaitForRabbitMQ(CancellationToken cancellationToken = default);
    }

    ///<summary>
    /// Wraps Rabbit MQ with some helper methods
    ///</summary>
    public class RabbitClient : IDisposable, IRabbitClient
    {
        private readonly ILogger<RabbitClient> logger;
        private readonly IOptionsMonitor<RabbitMQOptions> clientOptionsMonitor;

        private IConnection? connection;
        private IModel? channel;

        private string exchangeName = "";
        private string routingKey = "";

        public bool IsConnected { get; private set; }

        public event EventHandler<Message>? OnMessageReceived;

        public RabbitClient(ILogger<RabbitClient> logger, IOptionsMonitor<RabbitMQOptions> clientOptionsMonitor)
        {
            this.logger = logger;
            this.clientOptionsMonitor = clientOptionsMonitor;
        }

        public void Connect(string exchangeName, string routingKey)
        {
            RabbitMQOptions clientOptions = clientOptionsMonitor.CurrentValue;

            ConnectionFactory factory = new ConnectionFactory
            {
                Uri = new Uri(clientOptions.Uri)
            };

            this.exchangeName = exchangeName;
            this.routingKey = routingKey;

            logger.LogInformation("Connecting to {Url}", factory.Uri);

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            IsConnected = true;

            logger.LogInformation("Connected to {Url}", factory.Uri);

            channel.ExchangeDeclare(exchangeName, ExchangeType.Topic, true);

            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queueName,
                              exchange: exchangeName,
                              routingKey: routingKey);

            logger.LogInformation(" [*] {Name} ready", nameof(RabbitClient));

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                logger.LogTrace(" [*] Received {message}", message);

                var msg = new Message()
                {
                    Content = message,
                    Exchange = ea.Exchange,
                    Topic = ea.RoutingKey,
                };

                OnMessageReceived?.Invoke(this, msg);
            };

            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }

        public async Task TryConnect(string exchangeName, string routingKey, CancellationToken stoppingToken)
        {
            var random = new Random();

            int attempt = 0;

            while (!IsConnected)
            {
                try
                {
                    ++attempt;
                    Connect(exchangeName, routingKey);
                    break;
                }
                catch
                {
                    await random.WaitForExponentialDelay(attempt - 1, stoppingToken);
                }
            }
        }

        ///<summary>
        /// Wait for RabbitMQ to start up
        ///</summary>
        public async Task WaitForRabbitMQ(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Waiting for RabbitMQ");

            logger.LogInformation("Using RabbitMQOptions {@Options}", this.clientOptionsMonitor.CurrentValue);

            RabbitMQOptions clientOptions = clientOptionsMonitor.CurrentValue;

            await ProgramCommon.WaitForTcpConnection(
                clientOptions.Host,
                clientOptions.Port,
                maxAttempts: 6,
                cancellationToken);

            logger.LogInformation("RabbitMQ should be up!");
        }

        public void SendMessage(string message)
        {
            logger.LogInformation("Sending message '{Message}' to '{Exchange}:{Topic}'", message, exchangeName, routingKey);
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);
            channel?.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);
        }

        public void Disconnect()
        {
            channel?.Close();
            connection?.Close();
        }

        public void Dispose()
        {
            channel?.Dispose();
            connection?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
