namespace Package.ControllersTree.Addons.Abstractions
{
	public interface IOnDemandController<TResult>
	{
		bool ShouldControllerRun();
		TResult GetDefaultResult();
	}
}
