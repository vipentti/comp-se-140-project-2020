using Common.Enumerations;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace Unit.Tests
{
    public class EnumerationTests
    {
        public class ApplicationStateNames : TheoryData<string>
        {
            public ApplicationStateNames()
            {
                foreach (var state in Enumeration.GetAll<ApplicationState>())
                {
                    Add(state.Name);
                }
            }
        }

        [Theory]
        [ClassData(typeof(ApplicationStateNames))]
        public void ApplicationState_From_Name(string name)
        {
            var state = ApplicationState.FromName(name);
            state.Should().NotBeNull();
            state.Name.Should().Be(name);
        }

        [Fact]
        public void Enumeration_Should_Have_Implementors()
        {
            var types = Enumeration.GetEnumerationTypes(typeof(Enumeration).Assembly).ToList();

            types.Should().NotBeEmpty();
        }
    }
}
