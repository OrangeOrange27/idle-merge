namespace Features.Core.MergeSystem.MergeableObjects
{
    public interface IMergeableObject
    {
        IMergeableObject ResultObject { get; }
        void Merge(IMergeableObject target);
    }
}
