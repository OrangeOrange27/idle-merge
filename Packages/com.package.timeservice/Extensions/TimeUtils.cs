using System;
using UnityEngine;

namespace Common.TimeService
{
	public static class TimeUtils
	{
		public const int MillisecondsInSecond = 1000;
		public const double SecondsInMinute = 60.0;

		public static DateTime GetUnixEpochDateTime()
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		}

		public static double GetCurrentUtcTimeMilliseconds()
		{
			var centuryBegin = GetUnixEpochDateTime();
			var currentDate = DateTime.UtcNow;

			var elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
			var now = new TimeSpan(elapsedTicks);
			return now.TotalMilliseconds;
		}

		public static double GetCurrentUtcOffsetInSeconds()
		{
			return TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalSeconds;
		}

		public static DateTime ToDateTimeUtcFromEpochMilli(this long value)
		{
			return GetUnixEpochDateTime().AddMilliseconds(value);
		}
        
		public static DateTime ToDateTimeLocalFromEpochMilli(this long value)
		{
			return GetUnixEpochDateTime().AddMilliseconds(value).ToLocalTime();
		}
		
		public static long RoundTo(this long value, int roundTo)
		{
			return Convert.ToInt64(Mathf.Ceil((float)value / roundTo) * roundTo);
		}

		public static int RoundTo(this int value, int roundTo)
		{
			return Convert.ToInt32(Mathf.Ceil((float)value / roundTo) * roundTo);
		}
	}
}
