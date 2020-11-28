using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace APIGateway.Features.Messages
{
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService messageService;

        public MessagesController(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        [HttpGet]
        [Route("/messages")]
        public async Task<ActionResult<string>> GetMessages()
        {
            var messages = await messageService.GetMessages();

            return string.Join(Environment.NewLine, messages);
        }
    }
}
