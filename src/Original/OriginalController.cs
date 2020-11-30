using Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        public async Task<ActionResult<string>> Start()
        {
            // await instance.StartAsync(CancellationToken.None);
            await Task.Delay(0);
            instance.Resume();
            return ApplicationState.Running.ToString();
        }

        [HttpPut]
        [Route("/stop")]
        public async Task<ActionResult<string>> Stop()
        {
            //var source = new CancellationTokenSource(10 * 1000);
            // await instance.StopAsync(CancellationToken.None);
            await Task.Delay(0);
            instance.Pause();
            return ApplicationState.Paused.ToString();
        }

        [HttpPut]
        [Route("/reset")]
        public async Task<ActionResult<string>> Reset()
        {
            //await Stop();
            await Task.Delay(0);
            instance.Pause();
            instance.Reset();
            instance.Resume();
            return ApplicationState.Init.ToString();
        }

        [HttpPut]
        [Route("/state")]
        public async Task<ActionResult<string>> SetState(string state)
        {
            switch (ApplicationState.FromName(state))
            {
                case var it when it == ApplicationState.Init:
                {
                    return await Reset();
                }
                case var it when it == ApplicationState.Paused:
                {
                    return await Stop();
                }
                case var it when it == ApplicationState.Running:
                {
                    return await Start();
                }
                default:
                    break;
            }

            return state;
        }
    }
}
