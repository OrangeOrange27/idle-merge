using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Cysharp.Text;
using Demystifier;
using GameServices;
using Package.Logger.Abstraction;
#if SENTRY_INTEGRATION_ADDED
using Package.Logger.Sentry;
#endif
using Microsoft.Extensions.Logging;
using UnityEngine;
using ZLogger;
using Debug = UnityEngine.Debug;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Object = UnityEngine.Object;

namespace Package.Logger.ZLogger
{
	public class ZLogManager : ILogManager, ILogHandler
	{
		public const string LogFileName = "logs.log";
		public const string BackupLogFileName = "Backuplogs.log";

		private const LogLevel DefaultLogLevel = LogLevel.Debug;
		public const string UnhandledLogException = "Unhandled Debug.LogException";

		private readonly ILogger GlobalLogger;
		private readonly ILoggerFactory LoggerFactory;

		private readonly string LogFullFileName = Path.Combine(Application.persistentDataPath, LogFileName);

		private readonly string BackUpLogFullFileName =
			Path.Combine(Application.persistentDataPath, BackupLogFileName);

		private ILogHandler _unityLogHandler;

		public ZLogManager()
		{
#if UNITY_EDITOR
			const string extension = ".log";
			LogFullFileName = LogFullFileName.Replace(extension, Application.dataPath.GetHashCode() + extension);
			BackUpLogFullFileName =
				BackUpLogFullFileName.Replace(extension, Application.dataPath.GetHashCode() + extension);
#endif
            
			FileUtils.CopyFile(LogFullFileName, BackUpLogFullFileName);
			CleanLogFile();

			_unityLogHandler = Debug.unityLogger.logHandler;
			Debug.unityLogger.logHandler = this;
			
			// Standard LoggerFactory does not work on IL2CPP,
			// But you can use ZLogger's UnityLoggerFactory instead,
			// it works on IL2CPP, all platforms(includes mobile).
			LoggerFactory = UnityLoggerFactory.Create(
				builder =>
				{
					// ReSharper disable once RedundantAssignment
					var logLevel = DefaultLogLevel;
#if LOG_LEVEL_TRACE
					logLevel = LogLevel.Trace;
#endif
					builder.SetMinimumLevel(logLevel);
					builder.AddCrashlyticsUnityLogHandlerIntegration();
					builder.AddZLoggerUnityDebug(Options);
					builder.AddZLoggerFile(LogFullFileName, Options);
				});

			GlobalLogger = LoggerFactory.CreateLogger("NonZLog");

#if !UNITY_EDITOR
			Application.quitting += () => { LoggerFactory.Dispose(); };
#endif
		}


		public ILogger GetLogger(string categoryName)
		{
			return LoggerFactory.CreateLogger(categoryName);
		}

		public string GetLogs(long sizeOfChunkFromEnd = long.MaxValue)
		{
			return FileUtils.ReadFile(LogFullFileName, sizeOfChunkFromEnd);
		}

		public string GetBackupLogs(long sizeOfChunkFromEnd = long.MaxValue)
		{
			return FileUtils.ReadFile(BackUpLogFullFileName, sizeOfChunkFromEnd);
		}
		public void LogFormat(LogType type, Object context, string format, params object[] args)
		{
			const char startsValue = '[';

			if (args.Length > 0 && args[0].ToString().Count(ch => ch == startsValue) < 3)
			{
				GlobalLogger.ZLog(ToLogLevel(type), string.Format(format, args));
			}
			else
			{
				_unityLogHandler?.LogFormat(type, context, format, args);
			}
		}

		public void LogException(Exception exception, Object context)
		{
			GlobalLogger.ZLog(LogLevel.Error, exception, UnhandledLogException);
		}


		private LogLevel ToLogLevel(LogType unityLogType)
		{
			switch (unityLogType)
			{
				case LogType.Error:
				case LogType.Assert:
				case LogType.Exception:
					return LogLevel.Error;
				case LogType.Warning:
					return LogLevel.Warning;
				case LogType.Log:
					return LogLevel.Debug;
				default:
					throw new ArgumentOutOfRangeException(nameof(unityLogType), unityLogType, null);
			}
		}

		private void Options(ZLoggerOptions options)
		{
			var prefixFormat = ZString.PrepareUtf8<string, string, string>("[{0}] [{1}] [{2}] ");
			options.PrefixFormatter = (writer, info) =>
				prefixFormat.FormatTo(ref writer, info.Timestamp.ToString("dd/MM/yyyy HH:mm:ss.fff"),
					info.LogLevel.ToString(), info.CategoryName);
			var defaultExceptionHandler = options.ExceptionFormatter;
			options.ExceptionFormatter = (writer, exception) => defaultExceptionHandler(writer, exception.Demystify());
		}

		private void CleanLogFile()
		{
			using (var fs = new FileStream(LogFullFileName, FileMode.OpenOrCreate))
			{
				fs.SetLength(0);
			}
		}
	}
}
