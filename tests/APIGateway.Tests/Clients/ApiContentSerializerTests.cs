using APIGateway.Clients;
using Common;
using Common.Enumerations;
using Common.Messages;
using Common.States;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace APIGateway.Tests.Clients
{
    public class ApiContentSerializerTests
    {
        public class SerializeAsyncTests
        {
            private readonly ApiContentSerializer contentSerializer = new();

            [Theory]
            [ClassData(typeof(StateData))]
            public async Task SerializeAsync_Can_Serialize_Individual_ApplicationState(ApplicationState state)
            {
                var serialized = await contentSerializer.SerializeAsync(state);

                await CheckStringContent(serialized, content => content.Should().Be(state.ToString()));
            }

            [Theory]
            [ClassData(typeof(RunLogData))]
            public async Task SerializeAsync_Can_Serialize_Individual_RunLogEntry(RunLogEntry entry)
            {
                var serialized = await contentSerializer.SerializeAsync(entry);

                await CheckStringContent(serialized, content => content.Should().Be(entry.ToString()));
            }

            [Theory]
            [ClassData(typeof(TopicMessageData))]
            public async Task SerializeAsync_Can_Serialize_Individual_TopicMessage(TopicMessage entry)
            {
                var serialized = await contentSerializer.SerializeAsync(entry);

                await CheckStringContent(serialized, content => content.Should().Be(entry.ToString()));
            }

            [Fact]
            public async Task SerializeAsync_Can_Serialize_ListOf_RunLogEntries()
            {
                List<RunLogEntry> entries = TestLogEntries.ToList();

                var serialized = await contentSerializer.SerializeAsync(entries);

                await CheckStringContent(serialized, content => content.Should().Be(entries.RunLogEntriesToString()));
            }

            [Fact]
            public async Task SerializeAsync_Can_Serialize_ListOf_Messages()
            {
                var messages = TestTopicMessages.ToList();

                var serialized = await contentSerializer.SerializeAsync(messages);

                serialized.Should().NotBeNull();
                serialized.Should().BeOfType<StringContent>();
            }
        }

        public class DeserializeAsyncTests
        {
            private readonly ApiContentSerializer contentSerializer = new();

            [Theory]
            [ClassData(typeof(StateData))]
            public async Task DeserializeAsync_Can_Deserialize_Individual_ApplicationState(ApplicationState state)
            {
                var serialized = await contentSerializer.SerializeAsync(state);

                var deserialized = await contentSerializer.DeserializeAsync<ApplicationState>(serialized);

                deserialized.Should().Be(state);
            }

            [Theory]
            [ClassData(typeof(RunLogData))]
            public async Task DeserializeAsync_Can_Deserialize_Individual_RunLogEntry(RunLogEntry entry)
            {
                var serialized = await contentSerializer.SerializeAsync(entry);

                var deserialized = await contentSerializer.DeserializeAsync<RunLogEntry>(serialized);

                deserialized.Should().Be(entry);
            }

            [Theory]
            [ClassData(typeof(TopicMessageData))]
            public async Task DeserializeAsync_Can_Deserialize_Individual_TopicMessage(TopicMessage entry)
            {
                var serialized = await contentSerializer.SerializeAsync(entry);

                var deserialized = await contentSerializer.DeserializeAsync<TopicMessage>(serialized);

                deserialized.Should().Be(entry);
            }

            [Fact]
            public async Task DeserializeAsync_Can_Deserialize_ListOf_RunLogEntries()
            {
                List<RunLogEntry> entries = TestLogEntries.ToList();

                var serialized = await contentSerializer.SerializeAsync(entries);

                var deserialized = await contentSerializer.DeserializeAsync<IEnumerable<RunLogEntry>>(serialized);

                deserialized.Should().Equal(entries);
            }

            [Fact]
            public async Task DeserializeAsync_Can_Deserialize_ListOf_Messages()
            {
                var messages = TestTopicMessages.ToList();

                var serialized = await contentSerializer.SerializeAsync(messages);

                var deserialized = await contentSerializer.DeserializeAsync<IEnumerable<TopicMessage>>(serialized);

                deserialized.Should().Equal(messages);
            }
        }

        protected static async Task CheckStringContent(HttpContent serialized, Action<string> action = default)
        {
            serialized.Should().NotBeNull();
            serialized.Should().BeOfType<StringContent>();

            var content = await serialized.ReadAsStringAsync();

            action?.Invoke(content);
        }

        protected class RunLogData : TheoryData<RunLogEntry>
        {
            public RunLogData()
            {
                foreach (var entry in TestLogEntries)
                {
                    Add(entry);
                }
            }
        }

        protected class TopicMessageData : TheoryData<TopicMessage>
        {
            public TopicMessageData()
            {
                foreach (var entry in TestTopicMessages)
                {
                    Add(entry);
                }
            }
        }

        protected class StateData : TheoryData<ApplicationState>
        {
            public StateData()
            {
                foreach (var state in Enumeration.GetAll<ApplicationState>())
                {
                    Add(state);
                }
            }
        }

        protected static IEnumerable<RunLogEntry> TestLogEntries
        {
            get
            {
                foreach (var state in Enumeration.GetAll<ApplicationState>())
                {
                    yield return new RunLogEntry(TestUtils.Utils.GetDefaultTestTime(), state);
                }
            }
        }

        protected static IEnumerable<TopicMessage> TestTopicMessages
        {
            get
            {
                yield return new TopicMessage(TestUtils.Utils.GetDefaultTestTime(), "test-topic", "test-message");
            }
        }
    }
}
