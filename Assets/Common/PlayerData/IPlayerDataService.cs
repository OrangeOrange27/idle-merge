using System;
using Common.Authentication.Providers;
using Cysharp.Threading.Tasks;
using Features.Core.Placeables.Models;

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
        
        void GiveCollectible(CollectibleType type, int amount);
        void UseCollectible(CollectibleType type, int amount);
        
        PlayerSettingsData GetPlayerSettings();
    }
}