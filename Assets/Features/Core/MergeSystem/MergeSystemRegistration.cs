using Features.Core.MergeSystem.Config;
using Features.Core.MergeSystem.Controller;
using VContainer;

namespace Features.Core.MergeSystem
{
    public static class MergeSystemRegistration
    {
        public static void RegisterMergeSystem(this IContainerBuilder builder, MergeableObjectsConfigProvider mergeableObjectsConfigProvider)
        {
            builder.Register<IMergeController, MergeController>(Lifetime.Singleton);
            builder.RegisterInstance<IMergeableObjectsConfigProvider, MergeableObjectsConfigProvider>(mergeableObjectsConfigProvider);
            builder.RegisterInstance(mergeableObjectsConfigProvider.GetConfig());
        }
    }
}