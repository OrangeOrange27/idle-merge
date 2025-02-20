using System;
using System.Threading.Tasks;
using Demystifier;
using Firebase.Crashlytics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using ZLogger;

namespace GameServices
{
	[ProviderAlias("CrashlyticsUnityLogHandler")]
	public class CrashlyticsUnityLogHandlerProvider : ILoggerProvider
	{
		private readonly AddCrashlyticsUnityLogHandlerProcessor debugLogProcessor;

		public CrashlyticsUnityLogHandlerProvider(IOptions<ZLoggerOptions> options)
		{
			debugLogProcessor = new AddCrashlyticsUnityLogHandlerProcessor(options.Value);
		}

		public ILogger CreateLogger(string categoryName)
		{
			return new AsyncProcessZLogger(categoryName, debugLogProcessor);
		}

		public void Dispose()
		{
		}
	}

	public class AddCrashlyticsUnityLogHandlerProcessor : IAsyncLogProcessor
	{
		private readonly ZLoggerOptions _options;

		public AddCrashlyticsUnityLogHandlerProcessor(ZLoggerOptions options)
		{
			_options = options;
		}

		public ValueTask DisposeAsync()
		{
			return default;
		}

		public void Post(IZLoggerEntry log)
		{
			try
			{
				var msg = log.FormatToString(_options, null);

				Crashlytics.Log(msg);
				
				Exception exceptionToSend = default;
				if (log.LogInfo.Exception != null)
				{
					exceptionToSend = log.LogInfo.Exception;
				}
				else if (log.LogInfo.LogLevel == LogLevel.Error || log.LogInfo.LogLevel == LogLevel.Critical)
				{
					var withoutTimestamp = msg.Substring(msg.IndexOf(']') + 2);
					exceptionToSend = new LogErrorException(withoutTimestamp);
				}

				if (exceptionToSend != default)
				{
					if (string.IsNullOrEmpty(exceptionToSend.StackTrace))
					{
						//required to natively fill stacktrace for Crashlytics
						try
						{
							throw exceptionToSend;
						}
						catch (Exception e)
						{
							exceptionToSend = e;
						}
					}
					
					Crashlytics.LogException(exceptionToSend.Demystify());
				}
			}
			finally
			{
				log.Return();
			}
		}

		
	}

	public static class CrashlyticsUnityLogHandlerIntegrationExtension
	{
		public static void AddCrashlyticsUnityLogHandlerIntegration(this ILoggingBuilder builder)
		{
			builder.AddConfiguration();

			builder.Services.TryAddEnumerable(
				ServiceDescriptor.Singleton<ILoggerProvider, CrashlyticsUnityLogHandlerProvider>(x =>
					new CrashlyticsUnityLogHandlerProvider(x.GetService<IOptions<ZLoggerOptions>>())));
			LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, CrashlyticsUnityLogHandlerProvider>(builder.Services);
		}

		public static void AddCrashlyticsUnityLogHandlerIntegration(this ILoggingBuilder builder, Action<ZLoggerOptions> configure)
		{
			if (configure == null) throw new ArgumentNullException(nameof(configure));

			builder.AddCrashlyticsUnityLogHandlerIntegration();
			builder.Services.Configure(configure);
		}
	}

	public class LogErrorException : Exception
	{
		public LogErrorException(string message) : base(message)
		{
		}

		public LogErrorException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
