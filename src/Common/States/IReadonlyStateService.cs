using System.Threading.Tasks;

namespace Common.States
{
    public interface IReadonlyStateService
    {
        Task<ApplicationState> GetCurrentState();
    }
}
