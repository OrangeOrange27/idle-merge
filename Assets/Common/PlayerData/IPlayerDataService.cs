using System;
using Common.Authentication.Providers;
using Cysharp.Threading.Tasks;

namespace Common.PlayerData
{
    public interface IPlayerDataService
    {
        bool IsOnline { get; }
        bool IsSignedIn { get; }
        PlayerData PlayerData { get; }

        public event Action<PlayerBalanceAssetType, int> OnBalanceChanged;
        
        IDisposable Update();
        UniTask LoginWithProvider(AuthProvider provider);
        
        void GiveBalance(PlayerBalanceAssetType type, int amount);
        void SpendBalance(PlayerBalanceAssetType type, int amount);
        
        PlayerSettingsData GetPlayerSettings();
    }
}