using Common.Enumerations;
using Refit;
using System.Threading.Tasks;

namespace APIGateway.Clients
{
    public interface IStateApiService
    {
        [Get("/state")]
        Task<ApplicationState> GetCurrentState();

        [Put("/state")]
        Task<ApplicationState> SetCurrentState(ApplicationState state);
    }
}
