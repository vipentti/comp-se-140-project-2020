using Common.Messages;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIGateway.Clients
{
    public interface IMessageApiService
    {
        [Get("/messages")]
        Task<IEnumerable<TopicMessage>> GetMessages();
    }
}
