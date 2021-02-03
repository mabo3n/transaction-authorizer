using System;

namespace AuthorizerTests.Utils
{
    public static class DateTimeUtils
    {
        public static DateTime RandomBetween(
            DateTime start,
            DateTime end,
            Random randomizer = null
        )
        {
            randomizer = randomizer ?? new Random();

            var interval = (end - start).TotalSeconds;
            var randomIntervalFraction = randomizer.Next(0, (int)interval);
            return start + new TimeSpan(randomIntervalFraction);
        }
    }
}
