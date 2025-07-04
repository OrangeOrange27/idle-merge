using Features.Core.Placeables.Models;

namespace Features.Core.Common.Models
{
    public class CollectibleItemUIModel : IUIItemModel
    {
        public CollectibleType CollectibleType { get; set; }
        public int Amout { get; set; }
        
        public string GetViewKey()
        {
            return CollectibleType.ToString();
        }
    }
}