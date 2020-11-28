using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.Http;

namespace APIGateway.Features.Messages
{
    public class MessagesHandler : IRoute
    {
        private readonly IMessageService messageService;

        public HttpMethod Method { get; } = HttpMethod.Get;

        public string Pattern { get; } = "/messages";

        public MessagesHandler(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        public async Task Handle(HttpContext context)
        {
            IEnumerable<string> messages = await messageService.GetMessages();

            var messageString = string.Join(System.Environment.NewLine, messages);

            await context.Response.WriteAsync(messageString);
        }
    }
}
