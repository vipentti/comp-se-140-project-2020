using APIGateway.Features.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIGateway.Tests.Features.Messages
{
    public class TestMessageService : IMessageService
    {
        public Task<IEnumerable<string>> GetMessages()
        {
            return Task.FromResult<IEnumerable<string>>(new[] {
                "First",
                "Second",
            });
        }
    }
}
