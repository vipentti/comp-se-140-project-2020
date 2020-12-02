using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace APIGateway.Features.Statistics
{
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        [HttpGet]
        [Route("/node-statistic")]
        public async Task<ActionResult<string>> GetNodeStatistics()
        {
            await Task.Delay(0);
            return "";
        }
    }
}
