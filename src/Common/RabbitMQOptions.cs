
namespace Common
{
    public record RabbitMQOptions
    {
        public int Port { get; init; } = 5672;
        public string Host { get; init; } = "rabbitmq";
        public string Username { get; init; } = "rabbit_user";
        public string Password { get; init; } = "rabbit_pass";
        public string Uri => $"amqp://{Username}:{Password}@{Host}:{Port}";
    }
}
