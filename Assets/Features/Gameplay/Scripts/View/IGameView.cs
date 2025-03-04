using UnityEngine;
using VContainer;

namespace Features.Gameplay.View
{
    public interface IGameView
    {
        Camera Camera { get; }
        IGameAreaView GameAreaView { get; }
        IGameUIView GameUIView { get; }
        
        void Initialize(IObjectResolver resolver);
    }
}