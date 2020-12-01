using Common;
using Common.States;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIGateway.Features.States
{
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly IStateService stateService;

        public StateController(IStateService stateService)
        {
            this.stateService = stateService;
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
        public async Task<ActionResult<IEnumerable<RunLogEntry>>> GetRunLog()
        {
            var entries = await stateService.GetRunLogEntries();
            return Ok(entries);
        }

        [HttpPut]
        [Route("/reinit-log")]
        [Produces("text/plain")]
        [Consumes("text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RunLogEntry>>> ReinitRunLog([FromBody] ApplicationState state)
        {
            await stateService.ClearRunLogEntries();

            _ = await stateService.SetCurrentState(state);

            var entries = await stateService.GetRunLogEntries();
            return Ok(entries);
        }
    }
}
