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
            _factories = factories.ToDictionary(f => f.Create().ObjectType);
        }
        
        public PlaceableModel Create(PlaceableType objectType)
        {
            if (_factories.TryGetValue(objectType, out var factory))
            {
                return factory.Create();
            }

            throw new InvalidOperationException($"No factory found for {objectType}");
        }
    }
}