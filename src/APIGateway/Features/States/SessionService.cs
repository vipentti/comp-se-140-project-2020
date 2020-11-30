using Microsoft.AspNetCore.Http;

namespace APIGateway.Features.States
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor contextAccessor;

        public SessionService(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        public string SessionId => GetSessionId();

        private string GetSessionId()
        {
            var context = contextAccessor.HttpContext;

            if (context is null)
            {
                return "default";
            }

            if (context.Request.Headers.TryGetValue("X-Session-Id", out var session))
            {
                return session.ToString();
            }

            return "default";
        }
    }
}
