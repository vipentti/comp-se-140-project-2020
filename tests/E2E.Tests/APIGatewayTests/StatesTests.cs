using Common;
using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace E2E.Tests.APIGatewayTests
{
    public class StatesTests : TestBase, IAsyncLifetime
    {
        private string Endpoint => $"/state";

        public async Task InitializeAsync()
        {
            await SetCurrentState(ApplicationState.Init);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [E2EFact]
        public async Task Get_State_Returns_Ok()
        {
            var state = await GetCurrentState();
            state.Should().NotBeNull();
        }

        [E2EFact]
        public async Task Put_State_Returns_Ok()
        {
            await SetCurrentState(ApplicationState.Paused);
        }

        [E2EFact]
        public async Task Get_State_After_Put_Returns_Updated_State()
        {
            ApplicationState desiredState = ApplicationState.Paused;

            await SetCurrentState(desiredState);

            ApplicationState currentState = await GetCurrentState();

            currentState.Should().NotBeNull();
            currentState.Should().Be(desiredState);
        }

        private async Task<ApplicationState> GetCurrentState()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, Endpoint);

            using HttpResponseMessage response = await SendRequest(request);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();

            var currentState = ApplicationState.FromName(content);

            currentState.Should().NotBeNull();

            return currentState;
        }

        private async Task SetCurrentState(ApplicationState newState)
        {
            using var request = new HttpRequestMessage(HttpMethod.Put, Endpoint)
            {
                Content = new StringContent(newState.ToString()),
            };

            using HttpResponseMessage response = await SendRequest(request);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
