using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.States
{
    public interface ISessionService
    {
        string SessionId { get; }
    }

    public class DefaultSessionService : ISessionService
    {
        public string SessionId { get; } = "default";
    }
}
