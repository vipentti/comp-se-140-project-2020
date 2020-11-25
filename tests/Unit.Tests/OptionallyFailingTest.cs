using Microsoft.Extensions.Configuration;
using Xunit;
using FluentAssertions;

namespace Unit.Tests
{
    /// <summary>
    /// Provides the ability to trigger test failures for example in CI
    /// by setting specific configuration/environment variables to contain
    /// certain values
    /// </summary>
    public class OptionallyFailingTest
    {
        private readonly IConfigurationRoot configuration;

        public OptionallyFailingTest()
        {
            var configBuilder = new ConfigurationBuilder();

            Common.ProgramCommon.ConfigureApplication(configBuilder);

            configuration = configBuilder.Build();
        }

        [Theory]
        [InlineData("FailingTest", "FAIL")]
        public void Should_Fail_When_Configuration_Variable_Is_Set_To_Value(string configName, string failValue)
        {
            var value = configuration.GetValue<string>(configName);

            value.Should().NotBe(failValue, because: "Should fail when '{0}' is set to '{1}'", configName, failValue);
        }
    }
}
