using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu]
    public class GridConfig : ScriptableObject
    {
        public GridLayout.CellLayout Layout;
        public Tile TilePrefab;
        public Vector3 CellSize;
        public GridLayout.CellSwizzle GridSwizzle;
    }
    
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private Vector2Int _size;
        [SerializeField] private Vector2 _gap;
        [SerializeField] private GridConfig _config;

        private List<Tile> _tiles;

        private Camera _camera;
        private Vector3 _cameraPositionTarget;
        private float _cameraSizeTarget;
        private Vector3 _moveVel;
        private float _cameraSizeVel;
        
        private Vector2 _currentGap;
        private Vector2 _gapVel;

        private bool _requiresGeneration = true;
        private void OnValidate() => _requiresGeneration = true;
        private  void Awake()
        {
            _tiles = new List<Tile>();
            _camera = Camera.main;
            _currentGap = _gap;
        }
        
        private void LateUpdate()
        {
            if (Vector2.Distance(_currentGap, _gap) > 0.01f)
            {
                _currentGap = Vector2.SmoothDamp(_currentGap, _gap,ref _gapVel, 0.1f);
                _requiresGeneration = true;
            }
        
            if (_requiresGeneration) Generate();
            PositionCamera();
        }

        private void Generate()
        {
            ClearExistingTiles();
            
            var bounds = new Bounds();
            var coordinates = new List<Vector3Int>();
            
            _grid.cellLayout = _config.Layout;
            _grid.cellSize = _config.CellSize;
            if (_grid.cellLayout != GridLayout.CellLayout.Hexagon) _grid.cellGap = _currentGap;
            _grid.cellSwizzle = _config.GridSwizzle;

            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.y; y++)
                {
                    coordinates.Add(new Vector3Int(x, y));
                }
            }

            foreach (var coordinate in coordinates)
            {
                var tile = CreateTile(coordinate);
                _tiles.Add(tile);
                bounds.Encapsulate(tile.transform.position);
            }
            
            SetCamera(bounds);
            _requiresGeneration = false;
        }
        
        private void ClearExistingTiles()
        {
            if(_tiles == null) 
                return;
            
            foreach (var tile in _tiles)
            {
                Destroy(tile.gameObject);
            }
            _tiles.Clear();
        }
        
        private Tile CreateTile(Vector3Int coordinate)
        {
            var tile = Instantiate(_config.TilePrefab, _grid.GetCellCenterWorld(coordinate), Quaternion.identity);
            tile.name = $"Tile {coordinate.x}, {coordinate.y}, {coordinate.z}";
            return tile;
        }
        
        private void SetCamera(Bounds bounds)
        {
            bounds.Expand(2);

            var vertical = bounds.size.y;
            var horizontal = bounds.size.x * _camera.pixelHeight / _camera.pixelWidth;

            _cameraPositionTarget = bounds.center + Vector3.back;
            _cameraSizeTarget = Mathf.Max(horizontal, vertical) * 0.5f;
        }

        private void PositionCamera()
        {
            if (_camera == null) 
                return;
            
            _camera.transform.position =
                Vector3.SmoothDamp(_camera.transform.position, _cameraPositionTarget, ref _moveVel, 0.5f);
            _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, _cameraSizeTarget, ref _cameraSizeVel, 0.5f);
        }
    }
}