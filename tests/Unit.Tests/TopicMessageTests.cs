using Common;
using Common.Messages;
using FluentAssertions;
using Xunit;

namespace Unit.Tests
{
    public class TopicMessageTests
    {
        [Theory]
        [ClassData(typeof(Topics))]
        public void TopicMessage_Supports_RoundTrip_Parsing(TopicMessage topic)
        {
            var asString = topic.ToString();

            asString.Should().Be($"{topic.Timestamp.ToISO8601()} Topic {topic.Topic}: {topic.Content}");

            var fromString = TopicMessage.FromString(asString);

            fromString.Should().Be(topic);
        }

        internal class Topics : TheoryData<TopicMessage>
        {
            public Topics()
            {
                Add(new TopicMessage(TestUtils.Utils.GetDefaultTestTime(), "test-topic", "test-message"));
                Add(new TopicMessage(TestUtils.Utils.GetDefaultTestTime(), "test-topic with whitespace after ", " test-message with withspaces "));
                Add(new TopicMessage(TestUtils.Utils.GetDefaultTestTime(), " test-topic with whitespace before and after ", "   test-message with withspaces   "));
                Add(new TopicMessage(TestUtils.Utils.GetDefaultTestTime(), "topic with empty content", ""));
                Add(new TopicMessage(TestUtils.Utils.GetDefaultTestTime(), "", "content with empty topic"));
                Add(new TopicMessage(TestUtils.Utils.GetDefaultTestTime(), "", ""));
                Add(new TopicMessage(TestUtils.Utils.GetDefaultTestTime(), " ", " "));
            }
        }
    }
}
