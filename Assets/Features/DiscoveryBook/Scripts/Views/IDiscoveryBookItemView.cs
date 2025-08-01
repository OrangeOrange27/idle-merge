using Features.Core.Common.Views;

namespace Features.DiscoveryBook.Scripts.Views
{
    public interface IDiscoveryBookItemView : IItemView
    {
        void SetText(string text);
    }
}