using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace APIGateway.Features.Messages
{
    public class MessagesHandler : IRequestHandler
    {
        public Task Handle(HttpContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
