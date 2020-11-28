namespace APIGateway.Features.States
{
    public record ApplicationState(int Id, string Name)
    {
        public static readonly ApplicationState Init = new ApplicationState(0, "INIT");
        public static readonly ApplicationState Paused = new ApplicationState(1, "PAUSED");
        public static readonly ApplicationState Running = new ApplicationState(1, "RUNNING");
        public static readonly ApplicationState Shutdown = new ApplicationState(1, "SHUTDOWN");

        public override string ToString() => Name;
    }
}
