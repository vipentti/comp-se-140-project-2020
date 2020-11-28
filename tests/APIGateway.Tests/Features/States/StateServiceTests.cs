using APIGateway.Features.States;
using FluentAssertions;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestUtils;
using Xunit;

namespace APIGateway.Tests.Features.States
{
    public class StateServiceTests
    {
        private readonly StateService stateService;

        public StateServiceTests()
        {
            stateService = new StateService(
                new InMemoryRunLogService(),
                new TestDateTimeService()
                {
                    UtcNow = new System.DateTime(2020, 11, 26, 11, 30, 45, 0, System.DateTimeKind.Utc)
                }
            );
        }

        [Fact]
        public async Task SetCurrentState_Handles_Multiple_Updates_Correctly()
        {
            var t1 = Task.Run(ChangeStateMultipleTimes);
            var t2 = Task.Run(ChangeStateMultipleTimes);

            var res = await Task.WhenAll(t1, t2);

            res.Should().HaveCount(2);
            res[0].Should().Be(ApplicationState.Paused);
            res[1].Should().Be(ApplicationState.Paused);
        }

        private async Task<ApplicationState> ChangeStateMultipleTimes()
        {
            var transitions = new Dictionary<ApplicationState, ApplicationState>()
            {
                { ApplicationState.Init, ApplicationState.Running },
                { ApplicationState.Running, ApplicationState.Paused },
                { ApplicationState.Paused, ApplicationState.Shutdown },
                { ApplicationState.Shutdown, ApplicationState.Init },
            };

            for (var i = 0; i < 1000; ++i)
            {
                foreach (var (current, next) in transitions)
                {
                    _ = await stateService.SetCurrentState(next);
                }
            }

            await stateService.SetCurrentState(ApplicationState.Paused);

            return await stateService.GetCurrentState();
        }
    }
}
