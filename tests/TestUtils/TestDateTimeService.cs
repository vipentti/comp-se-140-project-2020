using System;
using Common;

namespace TestUtils
{
    public class TestDateTimeService : IDateTimeService
    {
        public DateTime UtcNow { get; init; }
    }
}
