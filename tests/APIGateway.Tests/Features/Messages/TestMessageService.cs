
using System.Collections.Generic;
using System.Threading.Tasks;
using APIGateway.Features.Messages;

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
