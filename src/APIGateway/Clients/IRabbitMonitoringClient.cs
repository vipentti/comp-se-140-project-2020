using APIGateway.Features.Statistics;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIGateway.Clients
{
    public interface IRabbitMonitoringClient
    {
        [Get("/api/nodes")]
        Task<IEnumerable<NodeStatistic>> GetNodeStatistics();
    }
}
