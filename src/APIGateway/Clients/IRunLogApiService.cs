using Common.Enumerations;
using Common.States;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

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
