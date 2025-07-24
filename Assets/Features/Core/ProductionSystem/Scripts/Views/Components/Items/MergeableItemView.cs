using Features.Core.Common.Views;
using UnityEngine;

namespace Features.Core.ProductionSystem.Components
{
    public class MergeableItemView : ItemView, IMergeableItemView
    {
        [SerializeField] private Transform[] _stages;
        
        public void SetStage(int stage)
        {
            for (var i = 0; i < _stages.Length; i++)
            {
                _stages[i].gameObject.SetActive(i + 1 == stage);
            }
        }
    }
}