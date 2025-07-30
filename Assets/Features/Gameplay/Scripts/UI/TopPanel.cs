using System;
using Common.PlayerData;
using Features.Core.ProgressionSystem;
using Features.Gameplay.Scripts.UI;
using UnityEngine;

public class TopPanel : MonoBehaviour
{
    [SerializeField] private PlayerLevelWidget _levelWidget;
    [SerializeField] private BalancePanel _coinsBalance;
    [SerializeField] private BalancePanel _gemsBalance;
    [SerializeField] private BalancePanel _energyBalance;

    private IPlayerDataService _playerDataService;
    private IProgressionManager _progressionManager;

    public void Initialize(IPlayerDataService playerDataService, IProgressionManager progressionManager)
    {
        _playerDataService = playerDataService;
        _progressionManager = progressionManager;

        _playerDataService.OnBalanceChanged += OnPlayerBalanceChanged;
        _progressionManager.OnLevelChanged += UpdateLevel;

        OnPlayerBalanceChanged(PlayerBalanceAssetType.Coins, _playerDataService.PlayerBalance.Coins);
        OnPlayerBalanceChanged(PlayerBalanceAssetType.Gems, _playerDataService.PlayerBalance.Gems);
        OnPlayerBalanceChanged(PlayerBalanceAssetType.Energy, _playerDataService.PlayerBalance.Energy);
        OnPlayerBalanceChanged(PlayerBalanceAssetType.Xp, _playerDataService.PlayerBalance.Experience);
    }

    private void OnPlayerBalanceChanged(PlayerBalanceAssetType assetType, int amount)
    {
        switch (assetType)
        {
            case PlayerBalanceAssetType.Coins:
                _coinsBalance.SetBalance(_playerDataService.PlayerBalance.Coins);
                break;
            case PlayerBalanceAssetType.Gems:
                _gemsBalance.SetBalance(_playerDataService.PlayerBalance.Gems);
                break;
            case PlayerBalanceAssetType.Energy:
                _energyBalance.SetBalance(_playerDataService.PlayerBalance.Energy);
                break;
            case PlayerBalanceAssetType.Xp:
                _levelWidget.UpdateXp(_playerDataService.PlayerBalance.Experience,
                    _progressionManager.NextLevelConfig.ExperienceNeeded);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(assetType), assetType, null);
        }
    }

    private void UpdateLevel(int level)
    {
        _levelWidget.UpdateLevel(level);
    }

    private void OnDestroy()
    {
        _playerDataService.OnBalanceChanged -= OnPlayerBalanceChanged;
        _progressionManager.OnLevelChanged -= UpdateLevel;
    }
}