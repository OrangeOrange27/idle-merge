using System;
using System.Linq;
using Package.ControllersTree;
using Package.ControllersTree.Abstractions;
using Package.ControllersTree.Addons.Abstractions;
using Package.ControllersTree.Addons.ExtensionPoints;
using Package.ControllersTree.Addons.Logging;
using Package.ControllersTree.Implementations;
using Package.ControllersTree.Settings;
using Package.Logger.Abstraction;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Common.EntryPoint
{
	public class CustomControllerSettings : IControllerSettings
	{
		private static readonly ILogger Logger = LogManager.GetLogger("ControllerLogger");
		private readonly IObjectResolver _resolver;

		public CustomControllerSettings(IObjectResolver resolver)
		{
			_resolver = resolver;
		}

		public IControllerRunner<TPayload, TResult> GetRunner<TController, TPayload, TResult>(IControllerRunnerBase parentControllerRunner,
			Func<TController> factory)
			where TController : IControllerWithPayloadAndReturn<TPayload, TResult>
		{
			var controller = factory.Invoke();
			if (controller == null)
				throw new ControllerException($"{typeof(TController).FullName} factory invoking returned null");

			return CreateRunner<TController, TPayload, TResult>(parentControllerRunner, controller);
		}

		public IControllerRunnerBase GetRunner(IControllerRunnerBase parentControllerRunner, Func<IBaseController> factory)
		{
			var instance = factory.Invoke();
			var baseType = instance.GetType();
			var genericTypes = baseType.GetInterfaces().First(inter=> inter.Name.Contains("IControllerWithPayloadAndReturn")).GenericTypeArguments;
			var generics = new Type[genericTypes.Length + 1];
			generics[0] = baseType;
			genericTypes.CopyTo(generics, 1);
			
			var mi = typeof(CustomControllerSettings).GetMethods().First(info => info.Name == nameof(CreateRunner));
			var fooRef = mi.MakeGenericMethod(generics);
			return (IControllerRunnerBase)fooRef.Invoke(this, new object[] { parentControllerRunner, instance });
		}

		public IControllerRunner<TPayload, TResult> CreateRunner<TController, TPayload, TResult>(IControllerRunnerBase parentControllerRunner,
			TController controller)
			where TController : IControllerWithPayloadAndReturn<TPayload, TResult>
		{
			return
				new LoggerControllerRunner<TPayload, TResult>(Logger,
					new ExtensionPointsControllerRunner<TController, TPayload, TResult>(
						_resolver,
						new OnDemandControllerRunner<TPayload, TResult>(
							controller,
							DefaultRunner()
						)
					)
				);

			ControllerRunner<TPayload, TResult> DefaultRunner()
			{
				return new ControllerRunner<TPayload, TResult>(
					parentControllerRunner?.PathInTree ?? string.Empty,
					controller,
					runner => new ControllerChildren(runner, this),
					new ControllerResources()
				);
			}
		}
	}
}
