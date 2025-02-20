using System.Collections.Generic;
using System.Linq;
using Core.GridSystem.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;
using TileBase = Core.GridSystem.Tiles.TileBase;

namespace Core.GridSystem
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private Tilemap _tilemap;

        private Dictionary<Vector3Int, TileBase> _validCells;
        private bool _isTilemapUpToDate;

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

        private void OnTilemapChanged(Tilemap tilemap, Tilemap.SyncTile[] syncTiles)
        {
            if(tilemap != _tilemap)
                return;

            _isTilemapUpToDate = false;
        }

        private void UpdateTilemap()
        {
            BuildValidCellsMap();
        }

        private void BuildValidCellsMap()
        {
            _validCells = new Dictionary<Vector3Int, TileBase>();
            var bounds = _tilemap.cellBounds;
            
            foreach (var position in bounds.allPositionsWithin)
            {
                if (!_tilemap.HasTile(position)) continue;
                
                var tile = _tilemap.GetTile(position);
                var tileData = new TileData();
                tile.GetTileData(position, _tilemap, ref tileData);

                TileBase tileObject = default;
                tileData.gameObject?.TryGetComponent(out tileObject);
                if (tileObject != null)
                    _validCells.Add(position, tileObject);
            }
        }
        
        private IGameAreaTile GetRandomFreeTile()
        {
            var freeTiles = _validCells
                .Where(pair => pair.Value is IGameAreaTile { IsOccupied: false })
                .Select(pair => pair.Value as IGameAreaTile)
                .ToList();
            
            var rand = new Random();
            return freeTiles[rand.Next(freeTiles.Count)];
        }
    }
}
