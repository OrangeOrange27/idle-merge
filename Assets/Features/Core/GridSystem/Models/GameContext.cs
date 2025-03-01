using System;
using ObservableCollections;

namespace Features.Core
{
    [Serializable]
    public class GameContext
    {
        public ObservableList<PlaceableModel> Placeables = new();
    }
}