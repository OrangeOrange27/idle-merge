using System;
using System.Threading;
using Common.DeviceInfo;
using Common.TimeService.Infrastructure;
using NUnit.Framework;
using VContainer;
using FluentAssertions;


namespace Common.TimeService.Tests
{
	[TestOf(typeof(TimeService))]
	public class TimeServiceTests : VContainerTestFixture
	{
		protected override void InstallDependencies(IContainerBuilder containerBuilder)
		{
			containerBuilder.Register<IDeviceInfo, StandaloneDeviceInfo>(Lifetime.Singleton);

			containerBuilder.RegisterTimeService();
		}

		[Test]
		public void GetCurrentUtcTime_NotSynced_ReturnsDevicesUtc()
		{
			Resolver.Resolve<ITimeService>().GetCurrentUtcTime().Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(0.5f));
		}

		[Test]
		public void GetCurrentUtcTime_Synced_ReturnsDevicesUtc()
		{
			var timeService = Resolver.Resolve<ITimeService>();
			timeService.SynchronizeWithServerResponseTime((long)TimeUtils.GetCurrentUtcTimeMilliseconds());
			timeService.GetCurrentUtcTime().Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(0.5f));
		}

		[Test]
		public void GetCurrentUtcTime_Synced_2SecDelay_ReturnsDevicesUtc()
		{
			var timeService = Resolver.Resolve<ITimeService>();
			const int delay = 2;
			timeService.SynchronizeWithServerResponseTime((long)(TimeUtils.GetCurrentUtcTimeMilliseconds() - delay * TimeUtils.MillisecondsInSecond));
			timeService.GetCurrentUtcTime().Should().BeCloseTo(DateTime.UtcNow - TimeSpan.FromSeconds(delay), TimeSpan.FromSeconds(0.5f));
		}

		[Test]
		public void GetCurrentUtcTime_SyncedOnCheater_ReturnsCorrectedUtc()
		{
			var timeService = Resolver.Resolve<ITimeService>();
			const int delay = 2;
			timeService.SynchronizeWithServerResponseTime((long)(TimeUtils.GetCurrentUtcTimeMilliseconds() - delay * TimeUtils.MillisecondsInSecond));
			timeService.GetCurrentUtcTime().Should().BeCloseTo(DateTime.UtcNow - TimeSpan.FromSeconds(delay), TimeSpan.FromSeconds(0.5f));
		}

		[Test]
		public void GetLocalTime_NotSynced_ReturnsLocalDeviceTime()
		{
			Resolver.Resolve<ITimeService>().GetLocalTime().Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
		}

		[Test]
		public void GetLocalTime_SyncedOnCheater_ReturnsLocalDeviceTime()
		{
			var timeService = Resolver.Resolve<ITimeService>();

			timeService.SynchronizeWithServerResponseTime((long)(TimeUtils.GetCurrentUtcTimeMilliseconds() - 10 * TimeUtils.MillisecondsInSecond));
		}

		[Test]
		public void SynchronizeWithServerResponseTime_NoCheater_ReturnTheSameTime()
		{
			var timeService = Resolver.Resolve<ITimeService>();
			timeService.GetCurrentUtcTime().Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(0.5f));
			timeService.SynchronizeWithServerResponseTime((long)TimeUtils.GetCurrentUtcTimeMilliseconds());
			timeService.GetCurrentUtcTime().Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(0.5f));
		}

		[Test]
		public void SynchronizeWithServerResponseTime_Cheater_ReturnCorrectedTime()
		{
			var timeService = Resolver.Resolve<ITimeService>();
			timeService.GetCurrentUtcTime().Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(0.5f));

			const int diffSec = 10;
			timeService.SynchronizeWithServerResponseTime((long)(TimeUtils.GetCurrentUtcTimeMilliseconds() - diffSec * TimeUtils.MillisecondsInSecond));
			timeService.GetCurrentUtcTime().Should().BeCloseTo(DateTime.UtcNow - TimeSpan.FromSeconds(diffSec), TimeSpan.FromSeconds(0.5f));
		}

		[Test]
		public void UpdateActualBootTime_NoCheater_TimeContinueIncreasing()
		{
			var timeService = Resolver.Resolve<ITimeService>();
			timeService.GetCurrentUtcTime().Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(0.5f));
			Thread.Sleep(50);
			timeService.UpdateActualBootTime(Resolver.Resolve<IDeviceInfo>().GetSystemUptime());
			timeService.GetCurrentUtcTime().Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(0.5f));
		}
	}
}
