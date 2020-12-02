using System.Text.Json.Serialization;

namespace APIGateway.Features.Statistics
{
    public record FlatQueueStatistic
    {
        [JsonPropertyName("name")]
        public string Name { get; init; } = "";

        [JsonPropertyName("messages_delivered_recently")]
        public int MessagesDeliveredRecently { get; init; }

        [JsonPropertyName("message_delivery_rate")]
        public decimal MessageDeliveryRate { get; init; }

        [JsonPropertyName("messages_published_recently")]
        public int MessagesPublishedRecently { get; init; }

        [JsonPropertyName("message_publish_rate")]
        public decimal MessagePublishRate { get; init; }

        public static FlatQueueStatistic From(QueueStatistic statistic)
        {
            return new FlatQueueStatistic
            {
                Name = statistic.Name,
                MessagesDeliveredRecently = statistic.MessageStats.MessagesDeliveredRecently,
                MessageDeliveryRate = statistic.MessageStats.MessageDeliveryRate.Rate,
                MessagesPublishedRecently = statistic.MessageStats.MessagesPublishedRecently,
                MessagePublishRate = statistic.MessageStats.MessagePublishRate.Rate,
            };
        }
    }

    /// <summary>
    /// Raw queue statistics returned from rabbitmq
    /// </summary>
    public record QueueStatistic
    {
        [JsonPropertyName("name")]
        public string Name { get; init; } = "";

        [JsonPropertyName("message_stats")]
        public MessageStats MessageStats { get; set; } = new();
    }

    public record WithRate
    {
        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }
    }

    public record MessageStats
    {
        [JsonPropertyName("deliver_get")]
        public int MessagesDeliveredRecently { get; init; }

        [JsonPropertyName("deliver_get_details")]
        public WithRate MessageDeliveryRate { get; init; } = new();

        [JsonPropertyName("publish")]
        public int MessagesPublishedRecently { get; init; }

        [JsonPropertyName("publish_details")]
        public WithRate MessagePublishRate { get; init; } = new();
    }
}
