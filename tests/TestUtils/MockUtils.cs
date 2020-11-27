using Common;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace TestUtils
{
    public static class MockUtils
    {
        public static Mock<IRabbitClient> CreateMockClient()
        {
            var _rabbitClientMock = new Mock<IRabbitClient>(MockBehavior.Strict);

            _rabbitClientMock.Setup(it => it.WaitForRabbitMQ(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _rabbitClientMock.Setup(it => it.TryConnect(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _rabbitClientMock.Setup(it => it.SendMessage(It.IsAny<string>()));

            return _rabbitClientMock;
        }
    }
}
