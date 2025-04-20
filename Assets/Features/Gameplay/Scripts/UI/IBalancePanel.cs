using System;

namespace Features.Gameplay.Scripts.UI
{
    public interface IBalancePanel
    {
        event Action OnButtonClicked;
        void SetBalance(int value);
    }
}