using Refit;
using System.Threading.Tasks;

namespace APIGateway.Features.Original
{
    public interface IOriginalService
    {
        [Put("/start")]
        Task<string> Start();

        [Put("/stop")]
        Task<string> Stop();

        [Put("/reset")]
        Task<string> Reset();

        [Put("/state")]
        Task<string> SetState(string state);
    }
}
