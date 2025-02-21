using System;
using Common.ApplicationLifecycleNotifier.Infrastructure;
using Package.Logger.Abstraction;
using UnityEngine;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Common.ApplicationLifecycleNotifier
{
	internal class ApplicationLifecycleNotifier : MonoBehaviour, IApplicationLifecycleNotifier
	{
		public event Action<bool> ApplicationPauses;
		public event Action<bool> ApplicationFocuses;
		public event Action ApplicationQuit;

		private static readonly ILogger _logger = LogManager.GetLogger<ApplicationLifecycleNotifier>();

		private void OnApplicationPause(bool isPaused)
		{
			_logger.ZLogInformation("OnApplicationPause isPaused {0}", isPaused);
			ApplicationPauses?.Invoke(isPaused);
		}

		private void OnApplicationFocus(bool isFocused)
		{
			_logger.ZLogInformation("OnApplicationFocus isFocused {0}", isFocused);
			ApplicationFocuses?.Invoke(isFocused);
		}

		private void OnApplicationQuit()
		{
			_logger.ZLogInformation("OnApplicationQuit");
			ApplicationQuit?.Invoke();
		}
	}
}
