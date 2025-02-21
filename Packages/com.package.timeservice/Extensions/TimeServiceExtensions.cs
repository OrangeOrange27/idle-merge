using System;
using Common.TimeService.Infrastructure;

namespace Common.TimeService.Extensions
{
	public static class TimeServiceExtensions
	{
		public static TimeSpan GetRemainTime(this ITimeService timeService, long endDateInMillisecFromEpoch)
		{
			return endDateInMillisecFromEpoch.ToDateTimeUtcFromEpochMilli() - timeService.GetCurrentUtcTime();
		}
	}
}