using System;
using Features.Core.Placeables.Models;

namespace Features.Core.Placeables.Factories
{
    public interface IPlaceablesFactory
    {
        PlaceableType FactoryType { get; }
        PlaceableModel Create(Enum objectType);
    }
    
    public interface IPlaceablesFactory<in T> : IPlaceablesFactory  where T : Enum
    {
        PlaceableModel Create(T type);
    }
}