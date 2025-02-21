using Package.ControllersTree.Abstractions;

namespace Package.ControllersTree.Addons.ExtensionPoints
{
	public interface ISubController<T, TPayload, TResult> : IControllerWithPayloadAndReturn<TPayload, TResult>
		where T : IControllerWithPayloadAndReturn<TPayload, TResult>
	{
	}
}
