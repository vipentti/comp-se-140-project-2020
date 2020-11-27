using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIGateway.Features.Messages
{
    public interface IMessageService
    {
        Task<IEnumerable<string>> GetMessages();
    }
}
