using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using Common.DataProvider.Storage.Infrastructure;
using Cysharp.Threading.Tasks;

namespace Common.DataProvider.Storage.Implementation.File
{
	public abstract class FileBaseDataStorage : AsyncBaseDataStorage
	{
		protected abstract string EndPath { get; }

		protected abstract string FileNamePattern { get; }

		private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphorePool =
			new ConcurrentDictionary<string, SemaphoreSlim>();

		protected override async UniTask SaveAsync(string key, string value)
		{
			var filePath = EndPath + GetFileFullName(key);

			if (string.IsNullOrEmpty(value))
			{
				if (System.IO.File.Exists(filePath))
					System.IO.File.Delete(filePath);

				return;
			}

			var buffer = System.Text.Encoding.UTF8.GetBytes(value);

			using (var openFile = System.IO.File.Create(filePath))
			{
				await openFile.WriteAsync(buffer, 0, buffer.Length);
			}
		}

		protected override void Save(string key, string value)
		{
			var filePath = EndPath + GetFileFullName(key);

			if (string.IsNullOrEmpty(value))
			{
				if (System.IO.File.Exists(filePath))
					System.IO.File.Delete(filePath);

				return;
			}

			const int maxCharsPerChunk = 100;
			const int bytesInChar = 4;
			var totalCharsRead = 0;

			var buffer = new byte[maxCharsPerChunk * bytesInChar];

			using (var openFile = System.IO.File.Create(filePath))
			{
				while (totalCharsRead < value.Length)
				{
					var charsLeft = value.Length - totalCharsRead;
					var charsPerChunk = charsLeft > maxCharsPerChunk
						? maxCharsPerChunk
						: charsLeft;

					var bytesRead = System.Text.Encoding.UTF8.GetBytes(value, totalCharsRead, charsPerChunk, buffer, 0);

					if (bytesRead <= 0)
						return;

					openFile.Write(buffer, 0, bytesRead);

					totalCharsRead += charsPerChunk;
				}
			}
		}

		protected override SemaphoreSlim GetSemaphore(string key)
		{
			if (!_semaphorePool.ContainsKey(key))
			{
				_semaphorePool.TryAdd(key, new SemaphoreSlim(1));
			}

			return _semaphorePool[key];
		}

		protected override void DeleteAllInternal()
		{
			Directory.Delete(EndPath, true);
		}

		protected void LoadCached()
		{
			if (!Directory.Exists(EndPath))
				Directory.CreateDirectory(EndPath);

			var allFiles = Directory.GetFiles(EndPath, GetFileFullName("*"), SearchOption.TopDirectoryOnly);

			foreach (var file in allFiles)
			{
				var key = Path.GetFileNameWithoutExtension(Path.GetFileName(file));
				SaveDataToCache(key, LoadByName(file));
			}
		}

		protected virtual string GetFileFullName(string key)
		{
			return string.Format(FileNamePattern, key);
		}
	}
}