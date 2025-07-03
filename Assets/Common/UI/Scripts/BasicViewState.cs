using System.Threading;
using Cysharp.Threading.Tasks;
using Package.AssetProvider.ViewLoader.Infrastructure;
using Package.ControllersTree.Abstractions;
using Package.StateMachine;

namespace Common.UI
{
    public abstract class BasicViewState<TView> : BasicViewState<TView, EmptyPayloadType> where TView : IView
    {
        protected BasicViewState(ISharedViewLoader<TView> sharedViewLoader) : base(sharedViewLoader)
        {
        }

        protected abstract UniTask SetInitialViewState(TView view, CancellationToken token);

        protected override UniTask SetInitialViewState(EmptyPayloadType payload, TView view, CancellationToken token)
        {
            return SetInitialViewState(view, token);
        }
    }
    
    public abstract class BasicViewState<TView, TPayload> : IStateController<TPayload> where TView : IView
    {
        protected readonly ISharedViewLoader<TView> _sharedViewLoader;
        
        private readonly CancellationTokenSource _lifecycleCancellationTokenSource = new();

        protected readonly UniTaskCompletionSource<IStateMachineInstruction> CompletionSource = new();
        protected TView View;
        protected TPayload Payload;
        protected IControllerResources ControllerResources;
        protected IControllerChildren ControllerChildren;
        protected CancellationToken LifecycleCancellationToken => _lifecycleCancellationTokenSource.Token;

        protected BasicViewState(ISharedViewLoader<TView> sharedViewLoader)
        {
            _sharedViewLoader = sharedViewLoader;
        }

        public virtual UniTask OnInitialize(IControllerResources resources, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public virtual async UniTask OnStart(TPayload payload, IControllerResources resources, IControllerChildren controllerChildren, CancellationToken token)
        {
            Payload = payload;
            ControllerResources = resources;
            ControllerChildren = controllerChildren;
            View = await _sharedViewLoader.Load(resources, token, null);
            await SetInitialViewState(payload, View, token);
            View.OnCloseButtonPressedEvent += OnCloseButtonPressed;
            SubscribeOnInput(View);

            await ShowView(token);
        }

        public async UniTask<IStateMachineInstruction> Execute(IControllerResources resources, IControllerChildren controllerChildren, CancellationToken token)
        {
            return await CompletionSource.Task.AttachExternalCancellation(token);
        }

        public async UniTask OnStop(CancellationToken token)
        {
            View.OnCloseButtonPressedEvent -= OnCloseButtonPressed;
            UnsubscribeOnInput(View);
            _lifecycleCancellationTokenSource.Cancel();
            
            await HideView(token);
        }

        public virtual UniTask OnDispose(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        protected abstract UniTask SetInitialViewState(TPayload payload, TView view, CancellationToken token);

        protected abstract void SubscribeOnInput(TView view);

        protected abstract void UnsubscribeOnInput(TView view);
        
        protected virtual async UniTask ShowView(CancellationToken token)
        {
            View.ShowAsync(token).Forget();
        }
        
        protected virtual async UniTask HideView(CancellationToken token)
        {
            await View.HideAsync(token);
        }

        protected virtual void OnCloseButtonPressed()
        {
            GoBack();
        }
        
        protected virtual void GoBack()
        {
            CompletionSource.TrySetResult(StateMachineInstruction.GoBack);
        }
    }
}
