using System;
using System.Collections.Generic;
using System.Linq;
using Features.Core.Placeables.Models;

namespace Features.Core.Placeables.Factories
{
    public class PlaceablesFactoryResolver
    {
        private readonly Dictionary<PlaceableType, IPlaceablesFactory> _factories;
        
        public PlaceablesFactoryResolver(IEnumerable<IPlaceablesFactory> factories)
        {
            _factories = factories.ToDictionary(f => f.FactoryType);
        }
        
        public PlaceableModel Create(PlaceableType placeableType, Enum objectType)
        {
            if (_factories.TryGetValue(placeableType, out var factory))
            {
                return factory.Create(objectType);
            }

            throw new InvalidOperationException($"No factory found for {placeableType}");
        }
    }
}