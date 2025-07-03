using System;
using Common.PlayerData;
using Features.Core.ProgressionSystem.Models;

namespace Features.Core.ProgressionSystem
{
    public class ProgressionManager : IProgressionManager, IDisposable
    {
        private readonly IPlayerDataService _playerDataService;
        private readonly LevelModel[] _levels;
        
        private int _currentLevelIndex;

        private int CurrentXP => _playerDataService.PlayerBalance.Experience;

        public int CurrentLevel => _currentLevelIndex + 1;
        public LevelModel CurrentLevelConfig =>  _levels[_currentLevelIndex];
        public LevelModel NextLevelConfig => _levels[_currentLevelIndex + 1];
        
        public event Action<int> OnLevelChanged;

        public ProgressionManager(IPlayerDataService playerDataService, LevelsConfig levelsConfig)
        {
            _playerDataService = playerDataService;
            _levels = levelsConfig.Levels;

            _playerDataService.OnBalanceChanged += OnPlayerBalanceChanged;
        }

        public void UpdatePlayerLevel()
        {
            for (var i = 0; i < _levels.Length; i++)
            {
                if (_levels[i].ExperienceNeeded <= CurrentXP)
                {
                    _currentLevelIndex = i;
                }
                else
                {
                    return;
                }
            }
        }

        private void OnPlayerBalanceChanged(PlayerBalanceAssetType assetType, int amount)
        {
            if(assetType != PlayerBalanceAssetType.Xp)
                return;
            
            UpdatePlayerLevel();
        }

        public void Dispose()
        {
            _playerDataService.OnBalanceChanged -= OnPlayerBalanceChanged;
        }
    }
}