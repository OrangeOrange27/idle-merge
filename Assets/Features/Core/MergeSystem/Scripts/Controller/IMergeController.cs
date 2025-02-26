using System.Collections.Generic;
using Features.Core.MergeSystem.MergeableObjects;

namespace Features.Core.MergeSystem.Controller
{
    public interface IMergeController
    {
        IMergeableObject Merge(IEnumerable<IMergeableObject> mergeableObjects, IMergeableObject target);
    }
}