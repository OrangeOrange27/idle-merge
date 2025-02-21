# State Machine

## Overview
StateMachine is an addon for controllers tree and works on top of the controllers tree
Controllers tree allows to run many instances in parallel and state machine just rotate them to make sure that right now working only one
- Supports nested statemachines (layering)
- Saves history of state changing
- Able to go back to previous state
- Navigation happens by types, no enums, not bottle neck for module architecture
- Each state may be executed in any state machine
- States with payload
    - Returning back saves payload and reuse the same one
- Stack of future states. It allows to setup list of states to execute
- Have the same signature and infrastructure of controllers tree
- DI/non-DI friendly

## When to use
Perfectly fits for windows and controlling game state. All feature related logic spawns and manages inside one state

### Typical use cases
#### Multilayer
Having multilayer state machine. For example, RootStateMachine (has RootSceneState, BattleSceneState, MapSceneState) and each of state has own statemachine with other states like SettingsState, ShopState, OfferState

#### Initialization statemachine with many states
Amazing to controll execution order during initalization flow
```csharp
StateMachineInstruction.GoToMany(
	StateMachineInstruction.GoTo<FooState, TPayload, IStateMachineInstruction>(stateFactory, payload),
	StateMachineInstruction.GoTo<BarState, TPayload, IStateMachineInstruction>(stateFactory, payload),
	StateMachineInstruction.GoTo<BooState, TPayload, IStateMachineInstruction>(stateFactory, payload)
	//...
);
```
Every state will be executed one by one, and doesn't know about each one

## When don't use
Usually core gameplay is more active and require quick and often state changing and not require such lifecycle, history, and error handling. Good practice is having entry point to core gameplay in state

## How to use

### Quick start
```csharp
public class YourEntryPoint : ISimpleController
{
    public async UniTask<DummyType> Execute(IControllerResources resources, IControllerChildren controllerChildren,
        CancellationToken token)
    {
        var initialState = StateMachineInstruction.GoTo<FooState>(stateFactory);
        var stateMachineRunner = controllerChildren.Create<StateMachineController>(this, () => new StateMachineController(), initialState);
        await stateMachineRunner.Run(token);
    }
    
    //Others controller methods..
}
public class FooState : IStateController
{
    private readonly UniTaskCompletedSource<IStateMachineInstruction> _completedSource = new UniTaskCompletedSource<IStateMachineInstruction>();
    
    public async UniTask OnInitialize(IControllerResources resources, CancellationToken token)
    {
        //Preload resources/scenes/prefabs
    }

    public async UniTask OnStart(DummyType payload, IControllerResources resources,
        IControllerChildren controllerChildren,
        CancellationToken token)
    {
        //Spawn prefabs and attach them to resources
        //Show animations
        //Subscribe on buttons
    }
    
    public async UniTask<IStateMachineInstruction> Execute(IControllerResources resources, IControllerChildren controllerChildren,
        CancellationToken token)
    {
        //Pay attention that state will be active while this method wouldn't be ended
        return _completedSource.Task;
    }
    
    public async UniTask OnStop(CancellationToken token)
    {
        //Unsubscribe on clicks
        //Hide animations
    }

    public async UniTask OnDispose(CancellationToken token)
    {
        //Dispose unmanaged resources if they were not added in IControllerResources
    }
    
    private OnGoNextButtonClick(int payload)
    {
        _completedSource.TrySetResult(StateMachineInstruction.GoTo<BooState, int>(stateFactory, payload));
    }
    
    private OnCloseButtonClick()
    {
        _completedSource.TrySetResult(StateMachineInstruction.Back);
    }
}
```


### GoTo
`StateMachineInstruction.GoTo` is a common istruction to tell statemachine of the next state.
It supports payloads, and GoToMany states

### Exit
`StateMachineInstruction.Exit` makes statemachine to finish statemachine and won't run any other states

### ExitTo
`StateMachineInstruction.ExitTo(IStateMachineInstruction instructionToParentStateMachine)` makes the same as regular `StateMachineInstruction.Exit` only difference is that statemachine returns instructionToParentStateMachine as result own work.
It uses to manage multilayer statemachine from other layer.
- **RootStateMachine**
    - RootLobbyState
        - **LobbyStateMachine**
            - GameplayStartState - `StateMachineInstruction.ExitTo( StateMachineInstruction.GoTo<RootGameplayState>(...) )`
              Will result of
- **RootStateMachine**
    - RootGameplayState

### GoBack
Regular way of changing state is say to state machine the next state. Although, some states, like settings and shop may be opened from many places and to return back we should know who run this state. This solution leads to circular dependency of states and generate odd modification such states after every new entry points to these states.
For solve that issue was implemented history feature on statemachine side.
It enables you to call just `StateMachineInstruction.GoBack` and returns to previous state without any depenency

#### Ignore in history
In some cases you may not require of history for some transitions. Usually it's require to avoid looping of some states
Just add `.IgnoreInHistory()` to ignore transaction in history, ignore it during `StateMachineInstruction.GoBack`
```csharp
StateMachineInstruction.GoTo<FooState, TPayload, IStateMachineInstruction>(
	stateFactory, payload).IgnoreInHistory()
```

#### History size
History sizes configures per each statemachine. Take a look at "Customize statemachine" section
For capturing payload statemachie uses lambda method and it captures a context and payload itself. As result all linked to resources/context linked to payload wouldn't collected by garbage collector. And in some case you might be not needed history feature and for optimization purpose you may disable history by setup `MaxHistorySize` field to 1

### Customize statemachine
You can use default `StateMachineController` it has unlimited history.
If you want to limit history, then create a new class and override next field
```csharp
class StateMachineWithNoHistory : StateMachineController
{
	public StateMachineWithNoHistory()
	{
		MaxHistorySize = 1;
	}
}
```

### Exception handling
Beside of controllers tree, statemachine has custom wrapper for states exceptions and don't rethrow them up to callstack. Statemachine has event for exception happens during `Initalize`, `Start`, `Exceute` phase of states.
To process these exceptions you could pass custom `IStateMachineErrorHandler` during stateMachine Start
```csharp
public class CustomStateMachineErrorHandler : IStateMachineErrorHandler
{
    //...
}

await _stateMachineRunner.Initialize(cancellationToken);
await _stateMachineRunner.Start(
    new StateMachinePayload(
        StateMachineInstruction.GoTo<FooState>(_objectResolver), 
        errorHandler: new CustomStateMachineErrorHandler()),
    cancellationToken);
await _stateMachineRunner.Execute(cancellationToken);
```

> By default statemachine use DefaultStateMachineErrorHandler.
> DefaultStateMachineErrorHandler has default controllerstree behvaiour to kill controller in case of exception inside

### InternalAPI access
You can create some controllers that can have access to internal statemachine API
They are commonly uses for features that require to interupt and override current state in any time
#### Decorator creation
```csharp
public class StateMachineDecorator : IStateMachineDecorator
{
    //...
}

await _stateMachineRunner.Initialize(cancellationToken);
await _stateMachineRunner.Start(
    new StateMachinePayload(
        StateMachineInstruction.GoTo<FooState>(_objectResolver), 
        children => new[]
        {
            children.Create<FooDecorator, IStateMachineInternalApi, EmptyPayloadType>(_objectResolver)
        }),
    cancellationToken);
await _stateMachineRunner.Execute(cancellationToken);
```

#### Change current state right now (ChangeActiveState)
```csharp
public class StateMachineDecorator : IStateMachineDecorator
{
    private IStateMachineInternalApi _stateMachineApi;
    public UniTask OnStart(IStateMachineInternalApi payload, /*other args*/)
    {
        _stateMachineApi = payload;
        _someController.OnSomethingImportantHappen += OnImportantHappen;
    }
    
    public UniTask OnStop(IStateMachineInternalApi payload, /*other args*/)
    {
        _someController.OnSomethingImportantHappen -= OnImportantHappen;
    }
    
    private void OnImportantHappen()
    {
        _stateMachineApi.ChangeActiveState(StateMachineInstruction.GoTo<ImportantState>(_yourStateResolver));
    }
}
```

#### Change state after current state will finish (AddStateAsNext)
> If current state will return GoBack(), then added state will be the very next state to run
> If current state will return GoTo, Exit, or any other instruction excpet of GoBack, then state will be run after finishing all spawned branch of logc by current state

```csharp
public class StateMachineDecorator : IStateMachineDecorator
{
    private IStateMachineInternalApi _stateMachineApi;
    public UniTask OnStart(IStateMachineInternalApi payload, /*other args*/)
    {
        _stateMachineApi = payload;
        _someController.OnSomethingNotSoImportantHappen += OnNotSoImportantHappen;
    }
    
    public UniTask OnStop(IStateMachineInternalApi payload, /*other args*/)
    {
        _someController.OnSomethingNotSoImportantHappen -= OnNotSoImportantHappen;
    }
    
    private void OnNotSoImportantHappen()
    {
        _stateMachineApi.AddStateAsNext(StateMachineInstruction.GoTo<NotSoImportantState>(_yourStateResolver));
    }
}
```