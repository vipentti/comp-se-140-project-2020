using Common;
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
        private readonly IDateTimeService dateTime;

        public StateController(IStateService stateService, IRunLogService runLogService, IDateTimeService dateTime)
        {
            this.stateService = stateService;
            this.runLogService = runLogService;
            this.dateTime = dateTime;
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
            await runLogService.WriteStateChange(new RunLogEntry(dateTime.UtcNow, state));
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
            //await runLogService.WriteEntry(new RunLogEntry(dateTime.UtcNow, state));
            await runLogService.WriteStateChange(new RunLogEntry(dateTime.UtcNow, state));

            var entries = await runLogService.GetRunLogEntries();
            return string.Join(Environment.NewLine, entries.Select(it => it.ToString()));
        }
    }
}
