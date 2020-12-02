using APIGateway.Clients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace APIGateway.Features.Statistics
{
    [ApiController]
    [Produces("application/json")]
    public class StatisticsController : ControllerBase
    {
        private readonly IRabbitMonitoringClient rabbitMonitoring;

        public StatisticsController(IRabbitMonitoringClient rabbitMonitoring)
        {
            this.rabbitMonitoring = rabbitMonitoring;
        }

        [HttpGet]
        [Route("/node-statistic")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NodeStatistic>> GetNodeStatistics()
        {
            var stats = await rabbitMonitoring.GetNodeStatistics();

            var first = stats.FirstOrDefault();

            return first is not null ? Ok(first) : NotFound();
        }
    }
}
