using System;
using Common.PlayerData;
using Features.Core.ProgressionSystem;

namespace Features.Gameplay.Scripts.Models
{
    [Serializable]
    public class GameUIDTO
    {
        public IPlayerDataService PlayerDataService { get; set; }
        public IProgressionManager ProgressionManager { get; set; }
    }
}