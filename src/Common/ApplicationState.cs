namespace Common
{
    public abstract record ApplicationState : Enumeration
    {
        public static readonly ApplicationState Init = new InitState();
        public static readonly ApplicationState Paused = new PausedState();
        public static readonly ApplicationState Running = new RunningState();
        public static readonly ApplicationState Shutdown = new ShutdownState();
        public static readonly ApplicationState Unknown = new UnknownState();

        protected ApplicationState(int Id, string Name) : base(Id, Name)
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

        public record InitState : ApplicationState
        {
            public InitState() : base(0, "INIT")
            {
            }

            public override string ToString() => Name;
        }

        public record PausedState : ApplicationState
        {
            public PausedState() : base(1, "PAUSED")
            {
            }

            public override string ToString() => Name;
        }

        public record RunningState : ApplicationState
        {
            public RunningState() : base(2, "RUNNING")
            {
            }

            public override string ToString() => Name;
        }

        public record ShutdownState : ApplicationState
        {
            public ShutdownState() : base(3, "SHUTDOWN")
            {
            }

            public override string ToString() => Name;
        }

        public record UnknownState : ApplicationState
        {
            public UnknownState() : base(99, "UNKNOWN")
            {
            }

            public override string ToString() => Name;
        }
    }
}
