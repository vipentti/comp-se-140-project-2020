using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIGateway.Features.States
{
    public interface ISessionService
    {
        string SessionId { get; }
    }
}
