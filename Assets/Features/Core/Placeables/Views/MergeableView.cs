using UnityEngine;

namespace Features.Core.Placeables.Views
{
    public class MergeableView : PlaceableView
    {
        [SerializeField] private GameObject _stage1;
        [SerializeField] private GameObject _stage2;
        [SerializeField] private GameObject _stage3;
        
        public override void SetStage(int stage)
        {
            _stage1.SetActive(stage == 1);
            _stage2.SetActive(stage == 2);
            _stage3.SetActive(stage == 3);
        }
    }
}