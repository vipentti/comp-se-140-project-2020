using Common;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Intermediate.Tests
{
    public class IntermediateTests
    {
        private readonly Mock<IRabbitClient> _originalMock;
        private readonly Mock<IRabbitClient> _intermediateMock;
        private readonly Mock<ILogger<Intermediate>> _loggerMock;

        public IntermediateTests()
        {
            _loggerMock = new Mock<ILogger<Intermediate>>();
            _originalMock = TestUtils.MockUtils.CreateMockClient();
            _intermediateMock = TestUtils.MockUtils.CreateMockClient();
        }

        [Fact]
        public async Task Intermediate_Sends_Message_When_OnMessageReceived_IsRaised()
        {
            var options = TestUtils.Utils.GetTestOptions();

            // Arrange
            var service = new Intermediate(
                _originalMock.Object,
                _intermediateMock.Object,
                _loggerMock.Object,
                options
            );

            var testMessage = new Message
            {
                Content = "Test"
            };

            // Act

            // Raise the event
            _originalMock.Raise(it => it.OnMessageReceived += null, null, testMessage);

            // The handler waits before sending
            await Task.Delay(options.Value.IntermediateDelay + 100);

            // Assert
            _originalMock.VerifyAdd(it => it.OnMessageReceived += It.IsAny<EventHandler<Message>>());
            _intermediateMock.Verify(it => it.SendMessage(It.Is<string>(msg => msg == $"Got {testMessage.Content}")), Times.Exactly(1));
        }

        [Fact]
        public void Intermediate_Throws_When_Constructing_Using_Same_Client()
        {
            var act = FluentActions.Invoking(() => new Intermediate(
                _originalMock.Object,
                _originalMock.Object,
                _loggerMock.Object,
                TestUtils.Utils.GetTestOptions()
            ));

            act.Should().Throw<ArgumentException>().WithMessage("*cannot refer to the same object*");
        }
    }
}
