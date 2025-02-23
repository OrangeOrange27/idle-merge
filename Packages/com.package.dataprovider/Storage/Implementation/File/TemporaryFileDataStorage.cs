
namespace Common.DataProvider.Storage.Implementation.File
{
	public sealed class TemporaryFileDataStorage : FileBaseDataStorage
	{
#if DEVELOPMENT_BUILD && UNITY_EDITOR
		private const string LibraryPath = "/~Save/Temp/";
#elif UNITY_STANDALONE_WIN || (UNITY_EDITOR && !UNITY_EDITOR_OSX)
		private const string LibraryPath = @"\Library\Temp\";
#else
		private const string LibraryPath = "/Library/Temp/";
#endif

#if DEVELOPMENT_BUILD && UNITY_EDITOR
		private static readonly string RootPath = $"{UnityEngine.Application.dataPath}/..";
#elif (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		private static readonly string RootPath = $"{UnityEngine.Application.persistentDataPath}";
#else
		private static readonly string RootPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
#endif

		private const string FilePattern = "{0}.save";

		protected override string FileNamePattern => FilePattern;

		protected override string EndPath => RootPath + LibraryPath;

		public TemporaryFileDataStorage()
		{
			LoadCached();
		}
	}
}