using System;
using Common.Authentication.Providers;
using Common.DataProvider.Infrastructure;
using Common.Server;
using Cysharp.Threading.Tasks;
using fbg;
using Features.Core.Placeables.Models;
using Microsoft.Extensions.Logging;
using Package.Disposables;
using Package.Logger.Abstraction;
using Debug = UnityEngine.Debug;

namespace Common.PlayerData
{
    public class PlayerDataService : IPlayerDataService
    {
        private const string DataKey = "PlayerData";
        private static readonly ILogger Logger = LogManager.GetLogger(nameof(PlayerDataService));
        private static bool _isOnline;

        private readonly IDataProvider _playerDataProvider;

        public bool IsOnline => _isOnline;
        public bool IsSignedIn { get; private set; }
        public PlayerData PlayerData { get; private set; }

        public event Action<PlayerBalanceAssetType, int> OnBalanceChanged;

        public PlayerDataService(IDataProvider playerDataProvider)
        {
            _playerDataProvider = playerDataProvider;
        }

        public IDisposable Update()
        {
            return new Disposable(SavePlayerState);
        }

        public async UniTask LoginWithProvider(AuthProvider provider)
        {
            //no server yet
            SetOfflinePlayer();
            PlayerData.Balance.OnBalanceChanged += (type, amount) => OnBalanceChanged?.Invoke(type, amount);
        }

        public void GiveBalance(PlayerBalanceAssetType type, int amount)
        {
            if (amount < 0)
                return;

            using (Update())
                PlayerData.Balance.ChangeBalance(type, amount);
        }

        public void SpendBalance(PlayerBalanceAssetType type, int amount)
        {
            if (PlayerData.Balance.GetBalance(type) < amount)
                return;

            using (Update())
                PlayerData.Balance.ChangeBalance(type, -amount);
        }

        public void GiveCollectible(CollectibleType type, int amount)
        {
            if (amount < 0)
                return;

            using (Update())
                PlayerData.Balance.ChangeCollectibleAmount(type, amount);
        }

        public void UseCollectible(CollectibleType type, int amount)
        {
            if (PlayerData.Balance.GetCollectibleAmount(type) < amount)
                return;

            using (Update())
                PlayerData.Balance.ChangeCollectibleAmount(type, -amount);
        }

        public PlayerSettingsData GetPlayerSettings()
        {
            return PlayerData.Settings;
        }

        private void SetOfflinePlayer()
        {
            _isOnline = false;
            PlayerData = GetLocalPlayer();
            IsSignedIn = true;
        }

        private void SavePlayerState()
        {
            SaveLocalPlayer();

            if (_isOnline) _ = ServerAPI.Player.UpdatePlayerDataAsync(PlayerData, PlayerData.AuthToken);
        }

        private void SaveLocalPlayer()
        {
            _playerDataProvider.SetAsync(DataKey, PlayerData);
        }

        private PlayerData GetLocalPlayer()
        {
            var localPlayerData = _playerDataProvider.Get<PlayerData>(DataKey);
            return localPlayerData ?? PlayerData.CreateNew();
        }
    }
}