using Cysharp.Threading.Tasks;

namespace Common.EntryPoint.Initialize
{
	public interface IAfterAuthInitialize
	{
		UniTask InitializeAfterAuth();
	}
}
