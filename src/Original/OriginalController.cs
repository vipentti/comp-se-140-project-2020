using Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Original
{
    [ApiController]
    public class OriginalController : ControllerBase
    {
        private readonly Original instance;

        public OriginalController(Original instance)
        {
            this.instance = instance;
        }

        [HttpPut]
        [Route("/start")]
        public async Task<ActionResult<ApplicationState>> Start()
        {
            await Task.Delay(0);
            instance.Resume();
            return ApplicationState.Running;
        }

        [HttpPut]
        [Route("/stop")]
        public async Task<ActionResult<ApplicationState>> Stop()
        {
            await Task.Delay(0);
            instance.Pause();
            return ApplicationState.Paused;
        }

        [HttpPut]
        [Route("/reset")]
        public async Task<ActionResult<ApplicationState>> Reset()
        {
            await Task.Delay(0);
            instance.Pause();
            instance.Reset();
            instance.Resume();
            return ApplicationState.Init;
        }

        [HttpPut]
        [Route("/state")]
        public async Task<ActionResult<ApplicationState>> SetState([FromBody] ApplicationState state)
        {
            return state switch
            {
                var it when it == ApplicationState.Init => await Reset(),
                var it when it == ApplicationState.Paused => await Stop(),
                var it when it == ApplicationState.Running => await Start(),
                var it when it == ApplicationState.Shutdown => throw new NotImplementedException($"{state} is not yet implemented"),
                _ => throw new NotImplementedException($"{state} is not yet implemented"),
            };
        }
    }
}
