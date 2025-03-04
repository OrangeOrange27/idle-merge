using System;
using Common.Utils;
using Features.Core.GridSystem.Managers;
using Features.Core.Placeables;
using Features.Core.Placeables.Models;
using Features.Gameplay.View;
using UnityEngine;
using VContainer.Unity;

namespace Features.Core.PlacementSystem
{
    public class SelectionController : ISelectionController, ITickable, IDisposable
    {
        private readonly Func<IGameView> _gameViewGetter;
        private readonly IPlacementSystem _placementSystem;

        private PlaceableModel _selectedPlaceable;
        private Vector3 _returnPosition;
        private IGameView _gameView;

        private IGameView GameView => _gameView ??= _gameViewGetter.Invoke();
        private IGridManager GridManager => GameView.GameAreaView.GridManager;
        private Camera Camera => GameView.Camera;

        private PlaceableModel SelectedPlaceable
        {
            get
            {
                if (_selectedPlaceable == null)
                    return null;
                return _selectedPlaceable.IsDisposed ? null : _selectedPlaceable;
            }
            set => _selectedPlaceable = value;
        }

        public SelectionController(Func<IGameView> gameViewGetter, IPlacementSystem placementSystem)
        {
            _gameViewGetter = gameViewGetter;
            _placementSystem = placementSystem;
        }

        public event Action<PlaceableModel> OnSelect;
        public event Action<PlaceableModel> OnDeselect;

        public void SelectPlaceable(PlaceableModel placeable)
        {
            if (SelectedPlaceable != null)
                DeselectPlaceable(true);

            SelectedPlaceable = placeable;
            SelectedPlaceable.IsSelected.Value = true;
            _returnPosition = SelectedPlaceable.Position.Value;

            OnSelect?.Invoke(SelectedPlaceable);
        }

        //todo: add mobile controls support
        public void Tick()
        {
            if (SelectedPlaceable == null)
                return;

            if (Input.GetMouseButtonUp(0))
            {
                var returnToOriginalPosition = !_placementSystem.TryPlaceOnCell(SelectedPlaceable, InputToGridPosition(Input.mousePosition));
                DeselectPlaceable(returnToOriginalPosition);
                return;
            }

            SelectedPlaceable.Position.Value = GetCellCenterFromInput(Input.mousePosition);
        }

        private void DeselectPlaceable(bool returnToOriginalPosition)
        {
            if(SelectedPlaceable==null)
                return;
            
            if (returnToOriginalPosition)
                SelectedPlaceable.Position.Value = _returnPosition;

            SelectedPlaceable.IsSelected.Value = false;

            OnDeselect?.Invoke(SelectedPlaceable);
            SelectedPlaceable = null;
        }

        private Vector3Int InputToGridPosition(Vector3 inputPosition)
        {
            var worldPosition = Camera.ScreenToWorldPoint(inputPosition);
            var gridPosition = GridManager.Grid.WorldToCell(worldPosition);

            return gridPosition;
        }

        private Vector3 GetCellCenterFromInput(Vector3 inputPosition)
        {
            var worldPosition = Camera.ScreenToWorldPoint(inputPosition);
            var gridPosition =
                GridManager.Grid.GetCellCenterWorld(
                    (worldPosition + (Vector3)(Vector2.Scale(Constants.PlaceableOffset, ((Vector2)worldPosition).normalized) - (Vector2)Constants.PlaceableOffset))
                    .ToVector3Int());
            gridPosition.z = Constants.ZOffset;

            return gridPosition;
        }

        public void Dispose()
        {
            DeselectPlaceable(true);
        }
    }
}