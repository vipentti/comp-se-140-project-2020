namespace Common.RedisSupport
{
    public record RedisOptions
    {
        public string Host { get; init; } = "redis";
        public int Port { get; init; } = 6379;
    }
}
