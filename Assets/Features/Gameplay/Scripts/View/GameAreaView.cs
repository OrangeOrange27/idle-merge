using Features.Core.GridSystem.Managers;
using UnityEngine;

namespace Features.Gameplay.View
{
    public class GameAreaView : MonoBehaviour, IGameAreaView
    {
        [SerializeField] private GridManager _gridManager;
        
        public IGridManager GridManager => _gridManager;
    }
}