using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace APIGateway.Features.States
{
    public class StateGetHandler : IRequestHandler
    {
        private readonly IStateService stateService;

        public StateGetHandler(IStateService stateService)
        {
            this.stateService = stateService;
        }

        public async Task Handle(HttpContext context)
        {
            var state = await stateService.GetCurrentState();
            await context.Response.WriteAsync(state.ToString());
        }
    }
}
