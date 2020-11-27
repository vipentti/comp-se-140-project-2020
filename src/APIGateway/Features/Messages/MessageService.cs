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

        public async Task<IEnumerable<string>> GetMessages()
        {
            var messageString = await httpClient.GetStringAsync("/messages");

            return messageString.Split(System.Environment.NewLine);
        }
    }
}
