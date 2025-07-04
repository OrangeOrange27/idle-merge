using UnityEngine;

namespace Features.Core.Common.Views
{
    public interface IItemView
    {
        GameObject GameObject { get; }
        void SetText(string text);
    }
}