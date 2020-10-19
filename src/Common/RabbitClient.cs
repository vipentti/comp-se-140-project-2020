
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common
{
    ///<summary>
    /// Wraps Rabbit MQ with some helper methods
    ///</summary>
    public class RabbitClient : IDisposable
    {
        private readonly ILogger<RabbitClient> logger;

        private IConnection? connection;
        private IModel? channel;

        private string exchangeName = "";
        private string routingKey = "";

        public bool IsConnected { get; private set; }

        public System.EventHandler<Message>? OnMessageReceived;

        public RabbitClient(ILogger<RabbitClient> logger)
        {
            this.logger = logger;
        }

        public void Connect(string uriString, string exchangeName, string routingKey)
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                Uri = new Uri(uriString)
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

        public async Task TryConnect(string uriString, string exchangeName, string routingKey, CancellationToken stoppingToken)
        {
            var random = new Random();

            int attempt = 0;

            while (!IsConnected)
            {
                try
                {
                    ++attempt;
                    Connect(uriString, exchangeName, routingKey);
                    break;
                }
                catch
                {
                    logger.LogInformation("Waiting for {Duration}", Constants.DelayBetweenConnectionAttempts);
                    await random.WaitForExponentialDelay(attempt - 1, stoppingToken);
                }
            }
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
