using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.States;
using Refit;

namespace APIGateway.Clients
{
    public interface IRunLogApiService
    {
        [Get("/run-log")]
        Task<IEnumerable<RunLogEntry>> GetRunLog();

        [Put("/reinit-log")]
        Task<IEnumerable<RunLogEntry>> ReinitRunLog(ApplicationState state);
    }
}
