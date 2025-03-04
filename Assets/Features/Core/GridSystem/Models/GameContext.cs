using System;
using Features.Core.Placeables.Models;
using ObservableCollections;

namespace Features.Core
{
    [Serializable]
    public class GameContext
    {
        public ObservableList<PlaceableModel> Placeables = new();
    }
}