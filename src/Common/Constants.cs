namespace Common
{
    public static class Constants
    {
        public const int RabbitPort = 5672;
        public const string RabbitHost = "rabbitmq";
        public const string RabbitUri = "amqp://rabbit_user:rabbit_pass@rabbitmq:5672";
        public const string ExchangeName = "ex-5-amqp-topic";
        public const string OriginalTopic = "my.o";
        public const string IntermediateTopic = "my.i";
        public const string AllMyTopics = "my.*";

        /// <summary>give rabbitmq time to start</summary>
        public const int StartupDelay = 1 * 1000;
        public const int DelayBetweenConnectionAttempts = 1 * 1000;
        public const int DelayAfterConnect = 1 * 1000;
        public const int IntermediateDelay = 1 * 1000;
        public const int LoopDelay = 10 * 1000;

        public const int DelayBetweenMessages = 3 * 1000;
        public const int MaximumNumberOfMessagesToSend = 3;
    }
}
