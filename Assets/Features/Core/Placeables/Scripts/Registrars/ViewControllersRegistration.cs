using Features.Core.Placeables.Models;
using Features.Core.Placeables.ViewController;
using Features.Core.Placeables.Views;
using VContainer;

namespace Features.Core.Placeables
{
    public static class ViewControllersRegistration
    {
        public static void RegisterViewControllers(this IContainerBuilder builder)
        {
            builder.Register(typeof(PlaceableViewController<>), Lifetime.Transient)
                .As(typeof(IPlaceableViewController<>));
            
            builder.Register(typeof(ViewControllerAdapter<>), Lifetime.Transient)
                .As(typeof(IPlaceableViewControllerBase));

            builder.RegisterFactory<PlaceableModel, IPlaceableViewControllerBase>(resolver =>
            {
                return (model) =>
                {
                    var type = model.GetType();

                    if (type == typeof(MergeableModel))
                        return Wrap(resolver.Resolve<IPlaceableViewController<MergeableModel>>());
                    if (type == typeof(CollectibleModel))
                        return Wrap(resolver.Resolve<IPlaceableViewController<CollectibleModel>>());
                    if (type == typeof(ProductionObjectModel))
                        return Wrap(resolver.Resolve<IPlaceableViewController<ProductionObjectModel>>());

                    return Wrap(resolver.Resolve<IPlaceableViewController<PlaceableModel>>());
                };
            }, Lifetime.Transient);
        }

        private static IPlaceableViewControllerBase Wrap<TModel>(IPlaceableViewController<TModel> generic)
            where TModel : PlaceableModel
        {
            return new ViewControllerAdapter<TModel>(generic);
        }
    }
}