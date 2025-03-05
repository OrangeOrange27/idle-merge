using Common.Authentication.Providers;
using Cysharp.Threading.Tasks;

namespace Common.PlayerData
{
    public interface IPlayerDataService
    {
        bool IsOnline { get; }
        bool IsSignedIn { get; }
        
        PlayerData PlayerData { get; }
        UniTask LoginWithProvider(AuthProvider provider);

    }
}