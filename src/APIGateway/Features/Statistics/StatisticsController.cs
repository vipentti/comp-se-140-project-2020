using APIGateway.Clients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIGateway.Features.Statistics
{
    [ApiController]
    [Produces("application/json")]
    public class StatisticsController : ControllerBase
    {
        private readonly ILogger<StatisticsController> logger;
        private readonly IRabbitMonitoringClient rabbitMonitoring;

        public StatisticsController(IRabbitMonitoringClient rabbitMonitoring, ILogger<StatisticsController> logger)
        {
            this.rabbitMonitoring = rabbitMonitoring;
            this.logger = logger;
        }

        [HttpGet]
        [Route("/node-statistic")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NodeStatistic>> GetNodeStatistics()
        {
            var stats = await rabbitMonitoring.GetNodeStatistics();

            logger.LogInformation("Received {@Stats}", stats);

            var first = stats.FirstOrDefault();

            return first is not null ? Ok(first) : NotFound();
        }

        [HttpGet]
        [Route("/queue-statistic")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FlatQueueStatistic>>> GetQueueStatistics()
        {
            var queues = await rabbitMonitoring.GetQueueStatistics();

            logger.LogInformation("Received {@Queues}", queues);

            return Ok(queues.Select(FlatQueueStatistic.From));
        }
    }
}
