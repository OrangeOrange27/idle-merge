using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Package.Logger.Abstraction
{
	public class DefaultUnityLogManager : ILogManager
	{
		public ILogger GetLogger<T>()
		{
			return new DefaultUnityLogger<T>(typeof(T).Name);
		}

		public ILogger GetLogger(string categoryName)
		{
			return new DefaultUnityLogger<string>(categoryName);
		}

		public string GetLogs(long sizeOfChunkFromEnd = long.MaxValue)
		{
			return string.Empty;
		}

		public string GetBackupLogs(long sizeOfChunkFromEnd = long.MaxValue)
		{
			return string.Empty;
		}
	}
}
