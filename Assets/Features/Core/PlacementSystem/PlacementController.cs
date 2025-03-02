using System;
using Common.Utils;
using Features.Core.GridSystem.Managers;
using Features.Gameplay.View;
using UnityEngine;
using VContainer.Unity;

namespace Features.Core.PlacementSystem
{
    public class PlacementController : IPlacementController, ITickable, IDisposable
    {
        private readonly Func<IGameView> _gameViewGetter;

        private IGameView _gameView;
        private PlaceableModel _selectedPlaceable;
        private Vector3 _savedPosition;

        private IGameView GameView => _gameView ??= _gameViewGetter.Invoke();
        private IGridManager GridManager => GameView.GameAreaView.GridManager;
        private Camera Camera => GameView.Camera;
        
        public PlacementController(Func<IGameView> gameViewGetter)
        {
            _gameViewGetter = gameViewGetter;
        }
        
        public void SelectPlaceable(PlaceableModel placeable)
        {
            if(_selectedPlaceable != null)
                DeSelectPlaceable();
            
            _selectedPlaceable = placeable;
            _selectedPlaceable.IsSelected.Value = true;
            _savedPosition = _selectedPlaceable.Position.Value;
        }
        
        public void DeSelectPlaceable()
        {
            _selectedPlaceable.Position.Value = _savedPosition;
            _selectedPlaceable.IsSelected.Value = false;
            _selectedPlaceable = null;
        }

        //todo: add mobile controls support
        public void Tick()
        {
            if (_selectedPlaceable == null)
                return;
            
            if (Input.GetMouseButtonUp(0))
            {
                DeSelectPlaceable();
                return;
            }
            
            var worldPosition = Camera.ScreenToWorldPoint(Input.mousePosition);
            var gridPosition = GridManager.Grid.GetCellCenterWorld(worldPosition.ToVector3Int());
            gridPosition.z = -1;
            _selectedPlaceable.Position.Value = gridPosition;
        }

        public void Dispose()
        {
            DeSelectPlaceable();
        }
    }
}