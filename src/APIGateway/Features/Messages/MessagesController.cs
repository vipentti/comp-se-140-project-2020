using APIGateway.Clients;
using Common.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIGateway.Features.Messages
{
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageApiService messageService;

        public MessagesController(IMessageApiService messageService)
        {
            this.messageService = messageService;
        }

        [HttpGet]
        [Route("/messages")]
        [Produces("text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TopicMessage>>> GetMessages()
        {
            var messages = await messageService.GetMessages();
            return Ok(messages);
        }
    }
}
