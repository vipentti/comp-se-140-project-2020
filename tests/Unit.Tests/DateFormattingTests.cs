using Xunit;
using Common;
using System;
using FluentAssertions;

namespace Unit.Tests
{
    public class DateFormattingTests
    {
        [Theory]
        [ClassData(typeof(ValidDateTimeTestData))]
        public void Should_Format_ToISO8601(DateTime input, string expected)
        {
            input.ToISO8601().Should().Be(expected);
        }

        [Theory]
        [ClassData(typeof(ConvertFromTestData))]
        public void Should_Convert_FromISO8601(string input, DateTime expected)
        {
            input.FromISO8601().Should().Be(expected);
        }

        [Theory]
        [InlineData("2020-11-26")]
        [InlineData("2020-11-26T13:00:45")]
        [InlineData("2020-11-26T13:00:45.11")]
        [InlineData("2020-11-26T13:00:45.111ö")]
        public void Should_Return_Null_When_Converting_FromISO8601(string invalidInput)
        {
            invalidInput.FromISO8601().Should().BeNull();
        }

        internal class ValidDateTimeTestData : TheoryData<DateTime, string>
        {
            public ValidDateTimeTestData()
            {
                Add(new DateTime(2020, 11, 26, 11, 30, 45, DateTimeKind.Utc), "2020-11-26T11:30:45.000Z");
                Add(new DateTime(2020, 11, 26, 11, 30, 45, DateTimeKind.Utc).AddMilliseconds(123), "2020-11-26T11:30:45.123Z");
                Add(new DateTime(2020, 11, 26, 11, 30, 45, DateTimeKind.Utc).AddMilliseconds(51), "2020-11-26T11:30:45.051Z");

                // Unspecified leaves the timezone out
                Add(new DateTime(2020, 11, 26, 11, 30, 45, DateTimeKind.Unspecified).AddMilliseconds(1), "2020-11-26T11:30:45.001");
            }
        }

        internal class ConvertFromTestData : TheoryData<string, DateTime>
        {
            public ConvertFromTestData()
            {
                Add("2020-11-26T11:30:45.000Z", new DateTime(2020, 11, 26, 11, 30, 45, DateTimeKind.Utc));
                Add("2020-11-26T11:30:45.123Z", new DateTime(2020, 11, 26, 11, 30, 45, DateTimeKind.Utc).AddMilliseconds(123));
                Add("2020-11-26T11:30:45.051Z", new DateTime(2020, 11, 26, 11, 30, 45, DateTimeKind.Utc).AddMilliseconds(51));

                // Unspecified leaves the timezone out
                Add("2020-11-26T11:30:45.001", new DateTime(2020, 11, 26, 11, 30, 45, DateTimeKind.Unspecified).AddMilliseconds(1));
            }
        }
    }
}
