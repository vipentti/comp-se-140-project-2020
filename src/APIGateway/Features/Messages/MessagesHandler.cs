using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace APIGateway.Features.Messages
{
    public class MessagesHandler : IRequestHandler
    {
        private readonly IMessageService messageService;

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
