using Microsoft.Extensions.Logging;

namespace Package.Logger.Abstraction
{
	public interface ILogManager
	{
		ILogger GetLogger(string categoryName);
		string GetLogs(long sizeOfChunkFromEnd = long.MaxValue);
		string GetBackupLogs(long sizeOfChunkFromEnd = long.MaxValue);
	}
}
