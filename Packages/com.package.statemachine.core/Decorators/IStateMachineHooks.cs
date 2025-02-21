using Cysharp.Threading.Tasks;
using Package.ControllersTree.Abstractions;

namespace Package.StateMachine.Decorators
{
	public interface IStateMachineHooks
	{
		UniTask OnBeforeStart(IControllerRunnerBase currentStateRunner);
		UniTask OnBeforeExecute(IControllerRunnerBase currentStateRunner);
		UniTask OnBeforeStop(IControllerRunnerBase currentStateRunner);
	}
}
