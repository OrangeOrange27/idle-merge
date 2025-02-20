using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using UnityEngine;

namespace Package.Logger.Abstraction
{
	public struct DefaultUnityLogger<T> : ILogger<T>
	{
		private readonly string _categoryName;

		public DefaultUnityLogger(string categoryName)
		{
			_categoryName = categoryName;
		}

		[SuppressMessage("Convention", "RA4:You should use ZLogger")]
		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
			Func<TState, Exception, string> formatter)
		{
			var logContent = $"[{_categoryName}] {formatter.Invoke(state, exception)}";

			switch (logLevel)
			{
				case LogLevel.Trace:
				case LogLevel.Debug:
				case LogLevel.Information:
					Debug.Log(logContent);
					break;
				case LogLevel.Warning:
					Debug.LogWarning(logContent);
					break;
				case LogLevel.Error:
				case LogLevel.Critical:
					Debug.LogError(logContent);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
			}
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return logLevel >= LogLevel.Debug;
		}

		public IDisposable BeginScope<TState>(TState state)
		{
			throw new NotImplementedException();
		}
	}
}
