namespace Features.Core
{
    public interface IFeatureUnlockManager
    {
        int GetFeatureUnlockLevel(string featureName);
    }
}