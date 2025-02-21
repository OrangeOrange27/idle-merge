using System;

namespace Common.TimeService.Infrastructure
{
	public interface ITimeService
	{
		DateTime GetCurrentUtcTime();
		DateTime GetCurrentTimeInLocalTimezone();
		DateTime GetLocalTime();
		double GetThreadSafeSinceStartSec();
		void UpdateActualBootTime(long uptime);
		void SynchronizeWithServerResponseTime(long serverUtcTime);
		bool IsTimeSynchronizedWithServer();
		void AddForwardTime(double time);
		void ResetForwardTime();
	}
}