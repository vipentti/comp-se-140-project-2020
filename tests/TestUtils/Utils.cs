using Common;
using Microsoft.Extensions.Options;

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
    }
}
