using System;
using System.Diagnostics;
using System.Threading;
using Common.DeviceInfo;
using Common.TimeService.Infrastructure;

namespace Common.TimeService
{
	public class TimeService : ITimeService
	{
		public event Action TimeSynchronizedWithServer;

		private const double OffsetNetworkLagThreshold = 3 * TimeUtils.SecondsInMinute;

		private readonly IDeviceInfo _deviceInfo;
		private readonly SynchronizationContext _context;

		private double _cachedBootTime;
		private double _actualBootTime;

		private double _serverTimeOffset;
		private double _forwardTimeOffset;

		private bool _serverTimeOffsetWasSet;
		private bool _hasPsfServerTime;

		private Stopwatch _timerSinceStartUp;


		public TimeService(IDeviceInfo deviceInfo)
		{
			_deviceInfo = deviceInfo;
			_context = SynchronizationContext.Current;

			SetInitialBootTime();
			SetTimerSinceStartUp();
		}

		private void SetTimerSinceStartUp()
		{
			_timerSinceStartUp = Stopwatch.StartNew();
		}

		public double GetThreadSafeSinceStartSec()
		{
			return (double)_timerSinceStartUp.ElapsedMilliseconds / TimeUtils.MillisecondsInSecond;
		}

		public DateTime GetCurrentUtcTime()
		{
			return DateTime.UtcNow.AddMilliseconds(
				_cachedBootTime - _actualBootTime +
			                        _serverTimeOffset +
				_forwardTimeOffset);
		}

		public DateTime GetCurrentTimeInLocalTimezone() => GetCurrentUtcTime().Add(TimeSpan.FromSeconds(TimeUtils.GetCurrentUtcOffsetInSeconds()));

		public DateTime GetLocalTime() => DateTime.Now;

		public void UpdateActualBootTime(long uptime)
		{
			_actualBootTime = (long)(TimeUtils.GetCurrentUtcTimeMilliseconds() - uptime);
		}

		public bool IsTimeSynchronizedWithServer()
		{
			return _serverTimeOffsetWasSet;
		}

		public void SynchronizeWithServerResponseTime(long serverUtcTime)
		{
			if (serverUtcTime <= 0)
			{
				return;
			}

			var serverTime = serverUtcTime;
			var clientTime = TimeUtils.GetCurrentUtcTimeMilliseconds();
			var serverTimeOffset = serverTime - clientTime;
			var serverTimeOffsetSeconds = Math.Round(serverTimeOffset);

			if (Math.Abs(serverTimeOffsetSeconds) < OffsetNetworkLagThreshold)
			{
				serverTimeOffsetSeconds = 0;
			}

			if (Math.Abs(serverTimeOffsetSeconds - _serverTimeOffset) > TimeUtils.SecondsInMinute || !_hasPsfServerTime)
			{
				SetServerTimeOffset(serverTimeOffsetSeconds);

				_cachedBootTime = _actualBootTime;

				_hasPsfServerTime = true;
			}
		}

		public void AddForwardTime(double time)
		{
			_forwardTimeOffset += time;

			if (_forwardTimeOffset < 0)
			{
				_forwardTimeOffset = 0;
			}
		}

		public void ResetForwardTime()
		{
			_forwardTimeOffset = 0;
		}

		private void InvokeOnUnityContext(Action callback)
		{
			if (_context != SynchronizationContext.Current)
			{
				_context.Send(
					_ => callback?.Invoke(),
					null);

				return;
			}

			callback?.Invoke();
		}

		private void SetInitialBootTime()
		{
			UpdateActualBootTime(_deviceInfo.GetSystemUptime());

			_cachedBootTime = _actualBootTime;
		}

		private void SetServerTimeOffset(double serverTimeOffset)
		{
			_serverTimeOffset = serverTimeOffset;
			_serverTimeOffsetWasSet = true;
			InvokeOnUnityContext(() =>
				TimeSynchronizedWithServer?.Invoke());
		}
	}
}