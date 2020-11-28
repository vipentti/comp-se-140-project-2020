using Common;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Original.Tests
{
    public class OriginalTests
    {
        private readonly Mock<IRabbitClient> _rabbitClientMock;
        private readonly Mock<ILogger<Original>> _loggerMock;

        public OriginalTests()
        {
            _rabbitClientMock = new Mock<IRabbitClient>(MockBehavior.Strict);
            _loggerMock = new Mock<ILogger<Original>>();

            _rabbitClientMock.Setup(it => it.WaitForRabbitMQ(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _rabbitClientMock.Setup(it => it.TryConnect(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _rabbitClientMock.Setup(it => it.SendMessage(It.IsAny<string>()));
        }

        [Fact]
        public async Task Original_Sends_Number_Of_Messages()
        {
            var options = TestUtils.Utils.GetTestOptions();
            // Arrange
            var original = new Original(_rabbitClientMock.Object, _loggerMock.Object, options);

            // Act
            await original.StartAsync(CancellationToken.None);

            // Give time for the service to run
            var config = options.Value;

            await Task.Delay(5 * config.DelayBetweenMessages + config.DelayAfterConnect);

            await original.StopAsync(CancellationToken.None);

            // Assert
            _rabbitClientMock.Verify(it => it.SendMessage(It.IsAny<string>()), Times.AtLeast(5));
        }
    }
}
