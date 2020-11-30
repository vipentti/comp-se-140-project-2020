using APIGateway.Features.Original;
using Common;
using FluentAssertions;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace APIGateway.Tests.Features.Original
{
    public class EnumerationSerializerTests
    {
        [Theory]
        [ClassData(typeof(StringData))]
        public async Task DeserializeAsync_Should_Serialize_StringContent_To_ApplicationState(string content)
        {
            var serializer = new EnumerationSerializer();

            var state = await serializer.DeserializeAsync<ApplicationState>(new StringContent(content));

            state.Should().NotBeNull();
            state.ToString().Should().Be(content);
        }

        [Theory]
        [ClassData(typeof(StateData))]
        public async Task SerializeAsync_Serializes_To_StringContent(ApplicationState state)
        {
            var serializer = new EnumerationSerializer();

            var content = await serializer.SerializeAsync(state);

            content.Should().NotBeNull();
            content.Should().BeOfType<StringContent>();
        }

        public class StringData : TheoryData<string>
        {
            public StringData()
            {
                Add(ApplicationState.Init.ToString());
                Add(ApplicationState.Paused.ToString());
                Add(ApplicationState.Running.ToString());
                Add(ApplicationState.Shutdown.ToString());
            }
        }

        public class StateData : TheoryData<ApplicationState>
        {
            public StateData()
            {
                Add(ApplicationState.Init);
                Add(ApplicationState.Paused);
                Add(ApplicationState.Running);
                Add(ApplicationState.Shutdown);
            }
        }
    }
}
