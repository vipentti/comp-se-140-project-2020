namespace Common.Options
{
    public record CommonOptions
    {
        public string ExchangeName { get; init; } = "ex-5-amqp-topic";
        public string OriginalTopic { get; init; } = "my.o";
        public string IntermediateTopic { get; init; } = "my.i";
        public string AllMyTopics { get; init; } = "my.*";

        public int StartupDelay { get; init; } = 1 * 1000;
        public int DelayBetweenConnectionAttempts { get; init; } = 1 * 1000;
        public int DelayAfterConnect { get; init; } = 2 * 1000;
        public int IntermediateDelay { get; init; } = 1 * 1000;
        public int LoopDelay { get; init; } = 10 * 1000;

        public int DelayBetweenMessages { get; init; } = 3 * 1000;
        public int MaximumNumberOfMessagesToSend { get; init; } = 3;
    }
}
