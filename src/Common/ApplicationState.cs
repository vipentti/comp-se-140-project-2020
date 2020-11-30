namespace Common
{
    public record ApplicationState : Enumeration
    {
        public static readonly ApplicationState Init = new(0, "INIT");
        public static readonly ApplicationState Paused = new(1, "PAUSED");
        public static readonly ApplicationState Running = new(2, "RUNNING");
        public static readonly ApplicationState Shutdown = new(3, "SHUTDOWN");
        public static readonly ApplicationState Unknown = new(99, "UNKNOWN");

        public ApplicationState(int Id, string Name) : base(Id, Name)
        {
        }

        public override string ToString() => Name;

        public static ApplicationState? FromName(string name)
        {
            try
            {
                return FromName<ApplicationState>(name);
            }
            catch
            {
                return null;
            }
        }
    }
}
