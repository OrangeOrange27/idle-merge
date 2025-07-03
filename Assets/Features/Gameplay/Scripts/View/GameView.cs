using Features.Gameplay.Scripts.Models;
using UnityEngine;
using VContainer;

namespace Features.Gameplay.View
{
    public class GameView : MonoBehaviour, IGameView
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private GameAreaView _gameAreaView;
        [SerializeField] private GameUIView _gameUIView;

        public Camera Camera => _camera;
        public IGameAreaView GameAreaView => _gameAreaView;
        public IGameUIView GameUIView => _gameUIView;

        public void Initialize(IObjectResolver resolver, GameUIDTO uiDto)
        {
            resolver.Inject(_gameAreaView.GridManager);
            _gameUIView.Initialize(uiDto);
        }
    }
}