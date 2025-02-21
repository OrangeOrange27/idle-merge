using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Package.ControllersTree;
using Package.Logger.Abstraction;
using UnityEngine;
using VContainer;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Common.EntryPoint
{
    public class EntryPoint : GameServicesInstaller
    {
        private static ILogger LoggerUnobservedException => LogManager.GetLogger("UniTask");
        private static readonly ILogger Logger = LogManager.GetLogger<EntryPoint>();
        
        [SerializeField] private bool _disableInitFlow;

        protected override void Awake()
        {
            InitializeUniTaskSettings();
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Input.multiTouchEnabled = false;

            base.Awake();

            if(!_disableInitFlow)
                Build();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            Logger.ZLogInformation("VContainer configure started");

            base.Configure(builder);
			
            if(!_disableInitFlow)
                builder.RegisterBuildCallback(resolver => Initialize(resolver).Forget());
        }

        private static async UniTask Initialize(IObjectResolver resolver)
        {
            var runner = ControllersTreeBootstrap.Create(resolver.Resolve<RootController>(), new CustomControllerSettings(resolver));
            await runner.Initialize(CancellationToken.None);
            await runner.Start(default, CancellationToken.None);
            runner.Execute(CancellationToken.None).Forget((_) =>
            {
                //mute exception because it will be processed in LoggerControllerRunner.cs
            });
        }
		
        private void OnUnobservedTaskException(Exception e)
        {
            if (e.IsOperationCanceledException() || e.GetBaseException().IsOperationCanceledException())
            {
#if UNITY_EDITOR
                LoggerUnobservedException.ZLogDebug(e, "Cancelled operation");
#endif
                return;
            }

            LoggerUnobservedException.ZLogError(e, "UnobservedException");
        }
		
        private void InitializeUniTaskSettings()
        {
            UniTaskScheduler.PropagateOperationCanceledException = true;
            UniTaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }
    }
}