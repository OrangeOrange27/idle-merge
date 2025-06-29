using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Features.Core.GridSystem.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Features.Core.GridSystem.Managers
{
    public class GridManager : MonoBehaviour, IGridManager
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private Tilemap _tilemap;

        private Dictionary<Vector3Int, GameAreaTile> _validCells;
        private bool _isTilemapUpToDate;

        public Grid Grid => _grid;
        
        public ReadOnlyDictionary<Vector3Int, GameAreaTile> ValidCells => new(_validCells);
        private bool NeedToUpdateValidCellsMap => _validCells == null || _isTilemapUpToDate == false;

        private void Awake()
        {
            Tilemap.tilemapTileChanged += OnTilemapChanged;
        }

        private void LateUpdate()
        {
            if (NeedToUpdateValidCellsMap)
                UpdateTilemap();
        }

        public IGameAreaTile GetRandomFreeTile()
        {
            var freeTiles = _validCells
                .Where(pair => !pair.Value.IsOccupied )
                .Select(pair => pair.Value)
                .ToList();

            if (freeTiles.Count <= 0)
                return null;
            
            var rnd = Random.Range(0, freeTiles.Count);
            var rndTile = freeTiles[rnd];
            return rndTile;
        }
        
        public IGameAreaTile GetTile(Vector3Int position)
        {
            return _validCells.GetValueOrDefault(position);
        }

        public IGameAreaTile[] GetNeighbours(Vector3Int position)
        {
            return GetNeighbours(GetTile(position));
        }

        public IGameAreaTile[] GetNeighbours(IGameAreaTile tile)
        {
            if(tile == null)
                return null;
            
            var neighbours = new List<IGameAreaTile>();
            var position = tile.Position;

            var left = GetTile(position + Vector3Int.left);
            if (left != null)
                neighbours.Add(left);

            var right = GetTile(position + Vector3Int.right);
            if (right != null)
                neighbours.Add(right);

            var up = GetTile(position + Vector3Int.up);
            if (up != null)
                neighbours.Add(up);

            var down = GetTile(position + Vector3Int.down);
            if (down != null)
                neighbours.Add(down);

            return neighbours.ToArray();
        }

        private void OnTilemapChanged(Tilemap tilemap, Tilemap.SyncTile[] syncTiles)
        {
            if(tilemap != _tilemap)
                return;

            _isTilemapUpToDate = false;
        }

        private void UpdateTilemap()
        {
            BuildValidCellsMap();

            _isTilemapUpToDate = true;
        }

        private void BuildValidCellsMap()
        {
            _validCells = new Dictionary<Vector3Int, GameAreaTile>();
            var bounds = _tilemap.cellBounds;

            foreach (var position in bounds.allPositionsWithin)
            {
                if (!_tilemap.HasTile(position))
                    continue;

                var tileObject = _tilemap.GetInstantiatedObject(position);
                if(tileObject == null)
                    continue;
                
                tileObject.transform.position = _tilemap.GetCellCenterWorld(position);
                
                tileObject.TryGetComponent(out GameAreaTile gameAreaTile);
                if (gameAreaTile != null)
                    _validCells.Add(position, gameAreaTile);
            }
        }
    }
}
