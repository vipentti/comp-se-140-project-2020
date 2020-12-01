using Common.Options;
using Microsoft.Extensions.Options;
using System;

namespace TestUtils
{
    public static class Utils
    {
        public static IOptions<CommonOptions> GetTestOptions()
        {
            return Options.Create(new CommonOptions
            {
                DelayAfterConnect = 0,
                DelayBetweenMessages = 100,
                IntermediateDelay = 100,
            });
        }

        public static DateTime GetDefaultTestTime() => new DateTime(2020, 11, 26, 11, 30, 45);
    }
}
