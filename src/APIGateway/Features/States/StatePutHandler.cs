using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace APIGateway.Features.States
{
    public class StatePutHandler : IRequestHandler
    {
        private readonly IStateService stateService;

        public StatePutHandler(IStateService stateService)
        {
            this.stateService = stateService;
        }

        public async Task Handle(HttpContext context)
        {
            context.Request.EnableBuffering();

            var req = context.Request;
            string bodyStr = "";

            // Arguments: Stream, Encoding, detect encoding, buffer size
            // AND, the most important: keep stream opened
            using (StreamReader reader
                    = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
            {
                bodyStr = await reader.ReadToEndAsync();
            }

            var currentState = await stateService.GetCurrentState();

            if (ApplicationState.FromName(bodyStr) is ApplicationState nextState)
            {
                currentState = nextState;
                _ = await stateService.SetCurrentState(currentState);
            }

            await context.Response.WriteAsync(currentState.ToString());
        }
    }
}
