using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace APIGateway.Features.States
{
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly IStateService stateService;
        private readonly IRunLogService runLogService;

        public StateController(IStateService stateService, IRunLogService runLogService)
        {
            this.stateService = stateService;
            this.runLogService = runLogService;
        }

        [HttpGet]
        [Route("/state")]
        [Produces("text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApplicationState>> GetCurrentState()
        {
            return await stateService.GetCurrentState();
        }

        [HttpPut]
        [Route("/state")]
        [Produces("text/plain")]
        [Consumes("text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApplicationState>> SetCurrentState([FromBody] ApplicationState state)
        {
            _ = await stateService.SetCurrentState(state);
            return state;
        }

        [HttpGet]
        [Route("/run-log")]
        [Produces("text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetRunLog()
        {
            var entries = await runLogService.GetRunLogEntries();
            return string.Join(Environment.NewLine, entries.Select(it => it.ToString()));
        }

        [HttpPut]
        [Route("/reinit-log")]
        [Produces("text/plain")]
        [Consumes("text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> ReinitRunLog([FromBody] ApplicationState state)
        {
            await runLogService.ClearRunLogEntries();

            _ = await stateService.SetCurrentState(state);

            var entries = await runLogService.GetRunLogEntries();
            return string.Join(Environment.NewLine, entries.Select(it => it.ToString()));
        }
    }
}
