using System;
using System.Threading;
using Common.ApplicationLifecycleNotifier.Infrastructure;
using Common.DeviceInfo;
using Common.TimeService.Infrastructure;
using Cysharp.Threading.Tasks;
using Package.ControllersTree.Abstractions;
using Package.ControllerTree;
using Package.TimeService;

namespace Common.TimeService
{
	public class UpdateBootTimeController : ISimpleController
	{
		private readonly TimeSpan _bootTimeUpdateFrequency = TimeSpan.FromSeconds(0.25f);

		private readonly IApplicationLifecycleNotifier _applicationLifecycleNotifier;
		private readonly IDeviceInfo _deviceInfo;
		private readonly ITimeService _timeService;

		public UpdateBootTimeController(
			IDeviceInfo deviceInfo,
			ITimeService timeService,
			IApplicationLifecycleNotifier applicationLifecycleNotifier)
		{
			_deviceInfo = deviceInfo;
			_timeService = timeService;
			_applicationLifecycleNotifier = applicationLifecycleNotifier;

			UpdateDeviceBootTime();
		}

		public UniTask OnInitialize(IControllerResources resources, CancellationToken token)
		{
			return UniTask.CompletedTask;
		}

		public UniTask OnStop(CancellationToken token)
		{
			_applicationLifecycleNotifier.ApplicationPauses -= OnApplicationPause;
			_applicationLifecycleNotifier.ApplicationFocuses -= OnApplicationFocus;
			return UniTask.CompletedTask;
		}

		public UniTask OnDispose(CancellationToken token)
		{
			return UniTask.CompletedTask;
		}

		public UniTask OnStart(EmptyPayloadType payload, IControllerResources resources, IControllerChildren controllerChildren, CancellationToken token)
		{
			_applicationLifecycleNotifier.ApplicationPauses += OnApplicationPause;
			_applicationLifecycleNotifier.ApplicationFocuses += OnApplicationFocus;
			return UniTask.CompletedTask;
		}

		public async UniTask<EmptyPayloadType> Execute(IControllerResources resources, IControllerChildren controllerChildren, CancellationToken token)
		{
			await UniTaskLoop.AsyncLoop(async () =>
				{
					UpdateDeviceBootTime();

					await UniTask.Delay(_bootTimeUpdateFrequency, cancellationToken: token);
				},
				token);

			return default;
		}

		private void OnApplicationFocus(bool isFocused)
		{
			UpdateDeviceBootTime();
		}

		private void OnApplicationPause(bool isPaused)
		{
			UpdateDeviceBootTime();
		}

		private void UpdateDeviceBootTime()
		{
			_timeService.UpdateActualBootTime(_deviceInfo.GetSystemUptime());
		}
	}
}
