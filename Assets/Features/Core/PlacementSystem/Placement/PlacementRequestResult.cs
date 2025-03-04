using Features.Core.Placeables.Models;
using UnityEngine;

namespace Features.Core.PlacementSystem
{
    public class PlacementRequestResult
    {
        public PlaceableModel Placeable { get; set; }
        public bool IsSuccessful { get; set; }
        public Vector3Int TargetCell { get; set; }
    }
}