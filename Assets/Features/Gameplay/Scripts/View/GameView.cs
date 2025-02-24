using UnityEngine;

namespace Features.Gameplay.View
{
    public class GameView : MonoBehaviour, IGameView
    {
        [SerializeField] private GameAreaView _gameAreaView;
        [SerializeField] private GameUIView _gameUIView;
        
        public IGameAreaView GameAreaView => _gameAreaView;
        public IGameUIView GameUIView => _gameUIView;
    }
}