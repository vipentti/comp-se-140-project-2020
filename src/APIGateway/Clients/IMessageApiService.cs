using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Messages;
using Refit;

namespace APIGateway.Clients
{
    public interface IMessageApiService
    {
        [Get("/messages")]
        Task<IEnumerable<TopicMessage>> GetMessages();
    }
}
