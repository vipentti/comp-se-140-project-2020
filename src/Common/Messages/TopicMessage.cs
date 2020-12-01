using System;

namespace Common.Messages
{
    /// <summary>
    /// A single message in the message log that is written to by the Observer
    /// </summary>
    public record TopicMessage(DateTime Timestamp, string Topic, string Content)
    {
        private const string TopicPrefix = "Topic ";
        private const string ContentPrefix = ": ";

        public static TopicMessage FromString(string input)
        {
            var indexOfTopic = input.IndexOf(TopicPrefix);

            if (indexOfTopic == -1)
            {
                throw new InvalidOperationException($"'{input}' is not valid {nameof(TopicMessage)}");
            }

            var indexOfContent = input.IndexOf(ContentPrefix, indexOfTopic + 1);

            if (indexOfContent == -1)
            {
                throw new InvalidOperationException($"'{input}' is not valid {nameof(TopicMessage)}");
            }

            var timestamp = input[0..indexOfTopic].Trim();

            var datetime = timestamp.FromISO8601() ?? throw new InvalidOperationException($"Invalid datetime {timestamp}");

            var topicStart = indexOfTopic + TopicPrefix.Length;

            var topic = input[topicStart..indexOfContent];

            var contentStart = indexOfContent + ContentPrefix.Length;

            // There should always be a whitespace between colon and the message
            var content = input[contentStart..];

            return new TopicMessage(
                datetime,
                topic,
                content
            );
        }

        public override string ToString() => $"{Timestamp.ToISO8601()} Topic {Topic}: {Content}";
    }
}
