using System;

namespace Common.Utils.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Checks if the given DateTime represents an older date than today, considering only the date.
        /// </summary>
        /// <param name="date">The DateTime to check.</param>
        /// <returns>True if the date is older than today, otherwise false.</returns>
        public static bool IsOlderThanToday(this DateTime date)
        {
            return date.Date < DateTime.Today;
        }
    }
}