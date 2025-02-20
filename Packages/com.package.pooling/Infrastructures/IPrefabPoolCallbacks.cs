namespace Package.Pooling.Infrastructures
{
	public interface IPrefabPoolCallbacks
	{
		void BeforeSpawn();
		void AfterReturnToPool();
	}
}
