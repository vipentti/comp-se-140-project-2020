using Common.Messages;
using Common.States;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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

        public static IEnumerable<(T item, int index)> Indexed<T>(this IEnumerable<T> self)
           => self.Select((item, index) => (item, index));

        public static DateTime? FromISO8601(this string input) =>
            DateTime.TryParseExact(input, ISO8601Format, null, DateTimeStyles.AdjustToUniversal, out var result)
                ? result
                : null;

        public static string RunLogEntriesToString(this IEnumerable<RunLogEntry> entries)
        {
            return string.Join(Environment.NewLine, entries.Select(it => it.ToString()));
        }

        public static string TopicMessagesToString(this IEnumerable<TopicMessage> entries)
        {
            return string.Join(Environment.NewLine, entries.Select(it => it.ToString()));
        }

        public static IEnumerable<RunLogEntry> RunLogEntriesFromString(this string input)
        {
            return input
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(RunLogEntry.FromString);
        }

        public static IEnumerable<TopicMessage> TopicMessagesFromString(this string input)
        {
            return input
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(TopicMessage.FromString);
        }
    }
}
