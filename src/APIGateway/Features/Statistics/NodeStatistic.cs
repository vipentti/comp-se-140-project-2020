using System.Text.Json.Serialization;

namespace APIGateway.Features.Statistics
{
    public record NodeStatistic
    {
        [JsonPropertyName("name")]
        public string Name { get; init; } = "";

        [JsonPropertyName("fd_used")]
        public int FdUsed { get; init; }

        [JsonPropertyName("mem_used")]
        public int MemoryUsed { get; init; }

        [JsonPropertyName("sockets_used")]
        public int SocketsUsed { get; init; }

        [JsonPropertyName("proc_used")]
        public int ProcessesUsed { get; init; }

        [JsonPropertyName("uptime")]
        public int Uptime { get; init; }
    }
}
