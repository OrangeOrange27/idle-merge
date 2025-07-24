using Features.Core.MergeSystem.Models;

namespace Features.Core.Common.Models
{
    public class MergeableItemUIModel : IUIItemModel
    {
        public MergeableType MergeableType { get; set; }
        public int Tier { get; set; }
        
        public string GetViewKey()
        {
            return MergeableType.ToString();
        }
    }
}