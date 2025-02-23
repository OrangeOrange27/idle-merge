using UnityEngine;

namespace Features.Core.MergeSystem.MergeableObjects
{
    public abstract class MergeableObject : GameAreaObject, IMergeableObject
    {
        [SerializeField] private MergeableObject _resultObject;

        public IMergeableObject ResultObject => _resultObject;
        
        public virtual void Merge(IMergeableObject target)
        {
            throw new System.NotImplementedException();
        }
    }
}