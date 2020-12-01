using APIGateway.Clients;
using APIGateway.Features.Messages;
using Common;
using Common.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIGateway.Tests.Features.Messages
{
    public class TestMessageService : IMessageService, IMessageApiService
    {
        private readonly IDateTimeService dateTime;

        public TestMessageService(IDateTimeService dateTime)
        {
            this.dateTime = dateTime;
        }

        public Task<IEnumerable<string>> GetMessages()
        {
            return Task.FromResult<IEnumerable<string>>(new[] {
                "First",
                "Second",
            });
        }

        Task<IEnumerable<TopicMessage>> IMessageApiService.GetMessages() => Task.FromResult<IEnumerable<TopicMessage>>(new[]
        {
            new TopicMessage(dateTime.UtcNow, "topic-1", "first"),
            new TopicMessage(dateTime.UtcNow, "topic-2", "second"),
        });
    }
}
