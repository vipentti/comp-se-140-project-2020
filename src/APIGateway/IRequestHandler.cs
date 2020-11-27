using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace APIGateway
{
    public interface IRequestHandler
    {
        Task Handle(HttpContext context);
    }
}
