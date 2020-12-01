using Common.Enumerations;
using FluentAssertions;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace APIGateway.Tests.Features.States
{
    public class StateTests : IClassFixture<APIGatewayAppFactory>
    {
        private readonly APIGatewayAppFactory factory;
        private readonly string endpoint = "/state";

        public StateTests(APIGatewayAppFactory factory)
        {
            this.factory = factory;
        }

        public class PutData : TheoryData<ApplicationState>
        {
            public PutData()
            {
                Add(ApplicationState.Init);
                Add(ApplicationState.Paused);
                Add(ApplicationState.Running);
                Add(ApplicationState.Shutdown);
            }
        }

        [Theory]
        [ClassData(typeof(PutData))]
        public async Task Put_State_Updates_Current_State(ApplicationState state)
        {
            // Arrange
            var client = factory.WithTestServices(services =>
            {
                services.SetupMockServices();
            }).CreateClient();

            var httpContent = new StringContent(state.ToString());

            // Act
            var response = await client.PutAsync(endpoint, httpContent);

            // Assert

            response.Should().NotBeNull();
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            content.Should().Be(state.ToString());
        }

        [Fact]
        public async Task Get_State_Returns_Current_State()
        {
            // Arrange
            var client = factory.WithTestServices(services =>
            {
                // Services...
                services.SetupMockServices();
            }).CreateClient();

            // Act

            var response = await client.GetAsync(endpoint);

            // Assert

            response.Should().NotBeNull();
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            content.Should().Be("INIT");
        }

        [Fact]
        public async Task Get_State_Returns_Success_StatusCode()
        {
            // Arrange
            var client = factory.WithTestServices(services =>
            {
                // Services...
                services.SetupMockServices();
            }).CreateClient();

            // Act

            var response = await client.GetAsync(endpoint);

            // Assert

            response.Should().NotBeNull();
            response.EnsureSuccessStatusCode();
        }
    }
}
