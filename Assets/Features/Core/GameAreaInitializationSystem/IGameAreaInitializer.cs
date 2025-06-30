using Cysharp.Threading.Tasks;

namespace Features.Core.GameAreaInitializationSystem
{
    public interface IGameAreaInitializer
    {
        UniTask InitializeGameArea(GameContext gameContext);
    }
}