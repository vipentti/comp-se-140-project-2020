using System;
using System.Globalization;

namespace Common
{
    public static class UtilityExtensions
    {
        public const string ISO8601Format = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK";

        /// <summary>
        /// <para>Returns the time in ISO8601 format</para>
        /// <para>Requires the given time to have UTC kind</para>
        /// <para>NOTE: Differs from regular "o" formatting by not having as many fraction digits</para>
        /// </summary>
        public static string ToISO8601(this DateTime time) => time.ToString(ISO8601Format);

        public static DateTime? FromISO8601(this string input) =>
            DateTime.TryParseExact(input, ISO8601Format, null, DateTimeStyles.AdjustToUniversal, out var result)
                ? result
                : null;
    }
}
