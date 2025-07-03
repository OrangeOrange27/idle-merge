using System;
using Features.Core.ProgressionSystem.Models;

namespace Features.Core.ProgressionSystem
{
    public interface IProgressionManager
    {
        int CurrentLevel { get; }
        LevelModel CurrentLevelConfig { get; }
        LevelModel NextLevelConfig { get; }
        
        event Action<int> OnLevelChanged;
        
        void UpdatePlayerLevel();
    }
}