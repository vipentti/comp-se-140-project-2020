using Common;
using System;

namespace TestUtils
{
    public class TestDateTimeService : IDateTimeService
    {
        public DateTime UtcNow { get; init; }
    }
}
