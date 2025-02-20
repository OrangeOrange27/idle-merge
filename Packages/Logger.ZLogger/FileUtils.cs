using System.IO;
using System.Text;
using UnityEngine;

namespace Package.Logger.ZLogger
{
	internal static class FileUtils
	{
		public static string ReadFile(string fileName, long sizeOfChunkFromEnd = long.MaxValue)
		{
			Debug.Log($"ReadFile {fileName} fileExist {File.Exists(fileName)}");

			if (!File.Exists(fileName))
			{
				return string.Empty;
			}

			using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (var sr = new StreamReader(fs, Encoding.Default))
				{
					if (fs.Length > sizeOfChunkFromEnd)
					{
						sr.BaseStream.Seek(fs.Length - sizeOfChunkFromEnd, SeekOrigin.Begin);
					}

					return sr.ReadToEnd();
				}
			}
		}

		public static void CopyFile(string originalFileName, string targetFileName)
		{
			if (File.Exists(originalFileName))
			{
				File.Copy(originalFileName, targetFileName, true);
			}
		}
	}
}
