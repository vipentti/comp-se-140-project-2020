using System;
using Xunit;
using Moq;
using FluentAssertions;
using Common;
using System.Threading.Tasks;
using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

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

            Common.ProgramCommon.ConfigureApplication(configBuilder);

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
            // Arrange
            var service = new Observer(
                _configuration,
                _clientMock.Object,
                _fileSystemMock.Object,
                _loggerMock.Object
            );

            var testMessage = new Message {
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
                    msg => msg.Contains($"Topic {testMessage.Topic}: {testMessage.Content}")
                )),
                Times.Exactly(1)
            );
        }
    }
}
