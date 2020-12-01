using System.Threading.Tasks;
using Common.Enumerations;
using Refit;

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
