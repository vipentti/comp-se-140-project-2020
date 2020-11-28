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
            var source = new CancellationTokenSource(10 * 1000);
            await instance.StartAsync(source.Token);
            return ApplicationState.Running.ToString();
        }

        [HttpPut]
        [Route("/stop")]
        public async Task<ActionResult<string>> Stop()
        {
            var source = new CancellationTokenSource(10 * 1000);
            await instance.StopAsync(source.Token);
            return ApplicationState.Paused.ToString();
        }

        [HttpPut]
        [Route("/reset")]
        public async Task<ActionResult<string>> Reset()
        {
            await Stop();
            instance.Reset();
            await Start();
            return ApplicationState.Init.ToString();
        }
    }
}
