using System;

namespace Common
{
    public interface IDateTimeService
    {
        DateTime UtcNow { get; }
    }
}
