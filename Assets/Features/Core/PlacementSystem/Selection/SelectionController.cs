using System;
using Common.Utils;
using Features.Core.GridSystem.Managers;
using Features.Gameplay.View;
using UnityEngine;
using VContainer.Unity;

namespace Features.Core.PlacementSystem
{
    public class SelectionController : ISelectionController, ITickable, IDisposable
    {
        private readonly Func<IGameView> _gameViewGetter;

        private IGameView _gameView;
        private PlaceableModel _selectedPlaceable;
        private Vector3 _returnPosition;

        private IGameView GameView => _gameView ??= _gameViewGetter.Invoke();
        private IGridManager GridManager => GameView.GameAreaView.GridManager;
        private Camera Camera => GameView.Camera;

        public SelectionController(Func<IGameView> gameViewGetter)
        {
            _gameViewGetter = gameViewGetter;
        }

        public event Action<PlaceableModel> OnSelect;
        public event Action<PlaceableModel> OnDeSelect;

        public void SelectPlaceable(PlaceableModel placeable)
        {
            if (_selectedPlaceable != null)
                DeSelectPlaceable(true);

            _selectedPlaceable = placeable;
            _selectedPlaceable.IsSelected.Value = true;
            _returnPosition = _selectedPlaceable.Position.Value;

            OnSelect?.Invoke(_selectedPlaceable);
        }

        //todo: add mobile controls support
        public void Tick()
        {
            if (_selectedPlaceable == null)
                return;

            if (Input.GetMouseButtonUp(0))
            {
                var returnToOriginalPosition = !TryPlaceOnCell(InputToGridPosition(Input.mousePosition));
                DeSelectPlaceable(returnToOriginalPosition);
                return;
            }

            _selectedPlaceable.Position.Value = GetCellCenterFromInput(Input.mousePosition);
        }

        private void DeSelectPlaceable(bool returnToOriginalPosition)
        {
            if (returnToOriginalPosition)
                _selectedPlaceable.Position.Value = _returnPosition;

            _selectedPlaceable.IsSelected.Value = false;

            OnDeSelect?.Invoke(_selectedPlaceable);
            _selectedPlaceable = null;
        }

        private bool TryPlaceOnCell(Vector3Int cellPosition)
        {
            var tile = GridManager.GetTile(cellPosition);
            if (tile == null || tile.IsOccupied)
                return false;

            _selectedPlaceable.ParentTile.Value = tile;
            return true;
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
            DeSelectPlaceable(true);
        }
    }
}