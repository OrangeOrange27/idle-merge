using System.Collections.Generic;
using System.Linq;
using Features.Core.MergeSystem.MergeableObjects;
using Microsoft.Extensions.Logging;
using Package.Logger.Abstraction;
using ZLogger;

namespace Features.Core.MergeSystem.Controller
{
    //todo: probably should add MergeRules in the future
    public class MergeController : IMergeController
    {
        private static readonly ILogger Logger = LogManager.GetLogger<MergeController>();

        public IMergeableObject Merge(IEnumerable<IMergeableObject> mergeableObjects, IMergeableObject target)
        {
            var enumerable = mergeableObjects as IMergeableObject[] ?? mergeableObjects.ToArray();

            if (ValidateMerge(enumerable) == false)
            {
                Logger.ZLogError("Trying to merge objects that are not the same type");
                return null;
            }

            foreach (var mergeableObject in enumerable)
            {
                mergeableObject.Merge(target);
            }

            return target.ResultObject;
        }

        private bool ValidateMerge(IMergeableObject[] enumerable)
        {
            if (!enumerable.Any())
                return false;

            var objectsType = enumerable.First().GetType();
            return enumerable.All(mergeableObject => mergeableObject.GetType() == objectsType);
        }
    }
}