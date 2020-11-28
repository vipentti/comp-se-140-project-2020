using Common;
using Refit;
using System.Threading.Tasks;

namespace APIGateway.Features.Original
{
    public interface IOriginalService
    {
        [Put("/start")]
        Task<ApplicationState> Start();

        [Put("/stop")]
        Task<ApplicationState> Stop();

        [Put("/reset")]
        Task<ApplicationState> Reset();
    }
}
