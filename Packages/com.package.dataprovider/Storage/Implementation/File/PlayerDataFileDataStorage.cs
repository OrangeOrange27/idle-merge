namespace Common.DataProvider.Storage.Implementation.File
{
	public sealed class PlayerDataFileDataStorage : FileBaseDataStorage
	{
#if UNITY_EDITOR
		private const string LibraryPath = "/~Save/PlayerData/";
#elif UNITY_STANDALONE_WIN || (UNITY_EDITOR && !UNITY_EDITOR_OSX)
		private const string LibraryPath = @"\Library\PlayerData\";
#else
		private const string LibraryPath = "/Library/PlayerData/";
#endif

#if UNITY_EDITOR
		private static readonly string RootPath = $"{UnityEngine.Application.dataPath}/..";
#else
		private static readonly string RootPath = $"{UnityEngine.Application.persistentDataPath}";
#endif

		private const string FilePattern = "{0}.save";

		protected override string FileNamePattern => FilePattern;

		protected override string EndPath => RootPath + LibraryPath;

		public PlayerDataFileDataStorage()
		{
			LoadCached();
		}
	}
}