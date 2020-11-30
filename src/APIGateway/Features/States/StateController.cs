using APIGateway.Features.Original;
using Common;
using Common.States;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace APIGateway.Features.States
{
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly ILogger<StateController> logger;
        private readonly IStateService stateService;
        private readonly IOriginalService originalService;

        public StateController(IStateService stateService, IOriginalService originalService, ILogger<StateController> logger)
        {
            this.stateService = stateService;
            this.originalService = originalService;
            this.logger = logger;
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

            //try
            //{
            //    await originalService.SetState(state);
            //}
            //catch (HttpRequestException ex)
            //{
            //    logger.LogWarning("Failed to set original state {@Exception}", ex);
            //}

            return state;
        }

        [HttpGet]
        [Route("/run-log")]
        [Produces("text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetRunLog()
        {
            var entries = await stateService.GetRunLogEntries();
            return string.Join(Environment.NewLine, entries.Select(it => it.ToString()));
        }

        [HttpPut]
        [Route("/reinit-log")]
        [Produces("text/plain")]
        [Consumes("text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> ReinitRunLog([FromBody] ApplicationState state)
        {
            await stateService.ClearRunLogEntries();

            _ = await stateService.SetCurrentState(state);

            //try
            //{
            //    await originalService.SetState(state);
            //}
            //catch (HttpRequestException ex)
            //{
            //    logger.LogWarning("Failed to set original state {@Exception}", ex);
            //}

            var entries = await stateService.GetRunLogEntries();

            return string.Join(Environment.NewLine, entries.Select(it => it.ToString()));
        }
    }
}
