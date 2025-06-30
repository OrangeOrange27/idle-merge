using UnityEngine;

namespace Features.Core.GameAreaInitializationSystem.Models
{
    [CreateAssetMenu(fileName = "GameAreaConfigSO", menuName = "Config/GameAreaConfig")]
    public class GameAreaConfigSO : ScriptableObject
    {
        public GameAreaConfig GameAreaConfig = new();
    }
}