using UnityEngine;
using Utils;

namespace Core.GridSystem.Tiles
{
    public class GameAreaTile : TileBase, IGameAreaTile
    {
        public bool IsOccupied => OccupyingObject != null;
        public IGameAreaPlaceable OccupyingObject => _occupyingObject;
        public Vector3Int Position => transform.position.ToVector3Int();

        private IGameAreaPlaceable _occupyingObject;
        
        public void Occupy(IGameAreaPlaceable gameAreaPlaceable)
        {
            _occupyingObject = gameAreaPlaceable;
        }
    }
}