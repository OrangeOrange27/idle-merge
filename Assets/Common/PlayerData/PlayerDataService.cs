using Common.Authentication.Providers;
using Cysharp.Threading.Tasks;

namespace Common.PlayerData
{
    public class PlayerDataService : IPlayerDataService
    {
        public bool IsOnline { get; }
        public bool IsSignedIn { get; }
        public PlayerData PlayerData { get; }
        
        public UniTask LoginWithProvider(AuthProvider provider)
        {
            throw new System.NotImplementedException();
        }
    }
}