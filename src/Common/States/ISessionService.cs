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
