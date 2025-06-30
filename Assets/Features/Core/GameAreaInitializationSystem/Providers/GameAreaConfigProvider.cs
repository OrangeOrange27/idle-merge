using Features.Core.GameAreaInitializationSystem.Models;
using UnityEngine;

namespace Features.Core.GameAreaInitializationSystem.Providers
{
    public class GameAreaConfigProvider : MonoBehaviour, IGameAreaConfigProvider
    {
        [SerializeField] private GameAreaConfigSO gameAreaConfigSO;
        public GameAreaConfig GetConfig()
        {
            return gameAreaConfigSO.GameAreaConfig;
        }
    }
}