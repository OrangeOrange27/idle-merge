using Package.ControllersTree.Abstractions;

namespace Package.ControllerTree
{
    public interface ISimpleController : IControllerWithPayloadAndReturn<EmptyPayloadType, EmptyPayloadType>
    {
    }

    public interface IControllerWithPayload<in TPayload> : IControllerWithPayloadAndReturn<TPayload, EmptyPayloadType>
    {
    }

    public interface IControllerWitResult<TResult> : IControllerWithPayloadAndReturn<EmptyPayloadType, TResult>
    {
    }
}