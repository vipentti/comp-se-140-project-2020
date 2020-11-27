using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;

namespace APIGateway.Features.Messages
{
    public class MessageService : IMessageService
    {
        private readonly HttpClient httpClient;

        public MessageService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task<IEnumerable<string>> GetMessages()
        {
            throw new System.NotImplementedException();
        }
    }
}
