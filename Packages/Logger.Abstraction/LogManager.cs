using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Package.Logger.Abstraction
{
	public class LogManager
	{
		private static ILogManager _logManager;

		public static void SetImplementation(ILogManager logManager)
		{
			_logManager = logManager;
		}

		public static ILogger GetLogger<T>()
		{
			return GetLogger(typeof(T).Name + string.Join(", ", typeof(T).GenericTypeArguments.Select(type => type.Name)));
		}

		public static ILogger GetLogger(string categoryName)
		{
			return GetLogManager().GetLogger(categoryName);
		}
		
		public static string GetLogs(long sizeOfChunkFromEnd = long.MaxValue)
		{
			return GetLogManager().GetLogs(sizeOfChunkFromEnd);
		}
		
		public static string GetBackupLogs(long sizeOfChunkFromEnd = long.MaxValue)
		{
			return GetLogManager().GetBackupLogs(sizeOfChunkFromEnd);
		}

		[SuppressMessage("Convention", "RA4:You should use ZLogger")]
		private static ILogManager GetLogManager()
		{
			if (_logManager == null)
			{
				_logManager = new DefaultUnityLogManager();
				Debug.LogWarning(
					"[LogManager] Wasn't setup own log implementation, use default one. For overwrite call SetImplementation before first log");
			}

			return _logManager;
		}
	}
}
