using System;
using Features.Core.GridSystem.Managers;
using Features.Core.Placeables;
using Features.Core.Placeables.Models;
using Features.Gameplay.View;
using Package.Logger.Abstraction;
using UnityEngine;
using VContainer.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Features.Core.PlacementSystem
{
    public class SelectionController : ISelectionController, ITickable, IDisposable
    {
        private const float MinDistanceToMoveSelectedObject = 0.15f;

        private static readonly ILogger Logger = LogManager.GetLogger<SelectionController>();
        private readonly Func<IGameView> _gameViewGetter;
        private readonly IPlacementSystem _placementSystem;

        private PlaceableModel _selectedPlaceable;
        private Vector3 _returnPosition;
        private IGameView _gameView;

        private IGameView GameView => _gameView ??= _gameViewGetter.Invoke();
        private IGridManager GridManager => GameView.GameAreaView.GridManager;
        private Camera Camera => GameView.Camera;

        private Vector2 _lastClickMousePosition;

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

            _lastClickMousePosition = Input.mousePosition;

            OnSelect?.Invoke(SelectedPlaceable);
        }

        //todo: add mobile controls support
        public void Tick()
        {
            if (SelectedPlaceable == null)
                return;

            if (Input.GetMouseButtonUp(0))
            {
                var returnToOriginalPosition =
                    !_placementSystem.TryPlaceOnCell(SelectedPlaceable, InputToGridPosition(Input.mousePosition));
                DeselectPlaceable(returnToOriginalPosition);
                return;
            }

            if (Vector2.Distance(_lastClickMousePosition, Input.mousePosition) > MinDistanceToMoveSelectedObject)
                SelectedPlaceable.Position.Value = GetCellCenterFromInput(Input.mousePosition);
        }

        private void DeselectPlaceable(bool returnToOriginalPosition)
        {
            if (SelectedPlaceable == null)
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
            inputPosition.z = Camera.WorldToScreenPoint(new Vector3(0, 0, PlaceablesConstants.ZOffset)).z;
            var worldPosition = Camera.ScreenToWorldPoint(inputPosition);

            var cellSize = GridManager.Grid.cellSize;
            var gridOrigin = GridManager.Grid.transform.position;

            var cellX = Mathf.FloorToInt((worldPosition.x - gridOrigin.x) / cellSize.x);
            var cellY = Mathf.FloorToInt((worldPosition.y - gridOrigin.y) / cellSize.y);
            var cellZ = 0;

            var flooredCellCoordinates = new Vector3Int(cellX, cellY, cellZ);

            var gridPosition = GridManager.Grid.GetCellCenterWorld(flooredCellCoordinates);
            gridPosition.z = PlaceablesConstants.ZOffset;

            return gridPosition;
        }

        public void Dispose()
        {
            DeselectPlaceable(true);
        }
    }
}