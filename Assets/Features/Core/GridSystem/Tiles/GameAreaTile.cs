using Common.Utils;
using Common.Utils.Extensions;
using Features.Core.Placeables.Models;
using UnityEngine;

namespace Features.Core.GridSystem.Tiles
{
    public class GameAreaTile : MonoBehaviour, IGameAreaTile
    {
        public bool IsOccupied => OccupyingObject != null;
        public PlaceableModel OccupyingObject => _occupyingObject;
        public Vector3Int Position => transform.position.ToVector3Int();
        public Transform Transform => transform;

        private PlaceableModel _occupyingObject;
        
        public void Occupy(PlaceableModel gameAreaPlaceable)
        {
            _occupyingObject = gameAreaPlaceable;
        }        
        public void DeOccupy()
        {
            _occupyingObject = null;
        }
    }
}