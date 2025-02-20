using Cysharp.Threading.Tasks;

namespace Common.EntryPoint.Initialize
{
	public interface IBeforeAuthInitialize
	{
		UniTask InitializeBeforeAuth();
	}
}
