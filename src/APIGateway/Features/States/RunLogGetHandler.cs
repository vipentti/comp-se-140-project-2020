using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace APIGateway.Features.States
{
    public class RunLogGetHandler : IRequestHandler
    {
        public RunLogGetHandler()
        {
        }

        public async Task Handle(HttpContext context)
        {
            await context.Response.WriteAsync("OK");
        }
    }
}
