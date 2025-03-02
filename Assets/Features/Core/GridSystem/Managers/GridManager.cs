using System.Collections.Generic;
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
