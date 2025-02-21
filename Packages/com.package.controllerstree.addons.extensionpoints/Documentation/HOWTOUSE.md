# How to use

## API

```csharp
public class LobbyIdleState : IStateController<EmptyPayloadType>  //techncially it's IControllerWithPayloadAndReturn<EmptyPayloadType, IStateMachineInstruction>
{
	//implementation
}

public class SubLobbyDrawFeatureState : ISubController<LobbyIdleState, EmptyPayloadType, IStateMachineInstruction> 
{
	//implementation of the same signature as LobbyIdleState has
	//Payload: EmptyPayloadType
	//Return: IStateMachineInstruction
}

builder.RegisterController<DrawFeatureState>();

```

> [!Note]
>
> You can inhereted as many ISubController as you wish for one SubController

Also, you can run `SubLobbyDrawFeatureState` as regular controller without any limitations

## Lifecycle

#### Initialise
```csharp
await UniTask.WhenAll(SubControllers.Intialize);
await parent.Intialize();
```

#### Start
> [!Note]
> SubControllers would have the same payload as the parent one

```csharp
await UniTask.WhenAll(SubControllers.Start (payload));
await parent.Start(payload);
```

#### Execute
> [!Warning]
> SubControllers can override result of `Execute` of parent controller. That was made to be able to override next instruction for statemachine from SubController

> [!note]
> You can return `default` if you have no logic in your SubController. This will not be accounted as valid result to override Parent's controller result

```csharp
await UniTask.WhenAny(SubControllers.Execute, parent.Execute);
```

#### Stop
The same as child controllers (please check main ControllerTree documentation)

#### Dispose
The same as child controllers (please check main ControllerTree documentation)


## Error handling
Any SubController can have exception on any of stage and it wouldn't affect parallel SubControllers or parent controller