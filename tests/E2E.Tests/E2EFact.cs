using System;
using Xunit;

namespace E2E.Tests
{
    public sealed class E2EFactAttribute : FactAttribute
    {
        public E2EFactAttribute()
        {
            if (!IsE2EEnvironment())
            {
                Skip = "Ignore when E2E environment variable has not been set.";
            }
        }

        private static bool IsE2EEnvironment()
        {
            var envvar = Environment.GetEnvironmentVariable("E2E");

            if (string.IsNullOrWhiteSpace(envvar))
            {
                return false;
            }

            try
            {
                return Convert.ToBoolean(envvar);
            }
            catch
            {
                return false;
            }
        }
    }
}
