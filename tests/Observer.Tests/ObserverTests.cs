using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO.Abstractions;
using Xunit;

namespace Observer.Tests
{
    public class ObserverTests
    {
        private readonly Mock<IRabbitClient> _clientMock;
        private readonly Mock<IFileSystem> _fileSystemMock;
        private readonly Mock<IFile> _fileMock;
        private readonly Mock<ILogger<Observer>> _loggerMock;
        private readonly IConfiguration _configuration;

        public ObserverTests()
        {
            var configBuilder = new ConfigurationBuilder();

            ProgramCommon.ConfigureApplication(configBuilder);

            _configuration = configBuilder.Build();

            _loggerMock = new Mock<ILogger<Observer>>();
            _clientMock = TestUtils.MockUtils.CreateMockClient();
            _fileSystemMock = new Mock<IFileSystem>();
            _fileMock = new Mock<IFile>();
            _fileSystemMock.Setup(it => it.File).Returns(_fileMock.Object);
        }

        [Fact]
        public void Observer_OnMessageReceived_Writes_Message_To_FileSystem()
        {
            var dateTime = new TestUtils.TestDateTimeService()
            {
                UtcNow = new DateTime(2020, 11, 28, 11, 30, 45)
            };

            // Arrange
            var service = new Observer(
                _configuration,
                _clientMock.Object,
                _fileSystemMock.Object,
                _loggerMock.Object,
                TestUtils.Utils.GetTestOptions(),
                dateTime
            );

            var testMessage = new Message
            {
                Topic = "test-topic",
                Content = "Test message"
            };

            // Act

            // Raise the event
            _clientMock.Raise(it => it.OnMessageReceived += null, null, testMessage);

            // Assert
            _clientMock.VerifyAdd(it => it.OnMessageReceived += It.IsAny<EventHandler<Message>>());

            _fileMock.Verify(it =>
                it.AppendAllText(It.IsAny<string>(), It.Is<string>(
                    // Remove trailing whitespace/newlines
                    msg => msg.TrimEnd().Equals($"{dateTime.UtcNow.ToISO8601()} Topic {testMessage.Topic}: {testMessage.Content}")
                )),
                Times.Exactly(1)
            );
        }
    }
}
