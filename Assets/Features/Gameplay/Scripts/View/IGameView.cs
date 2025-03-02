using UnityEngine;

namespace Features.Gameplay.View
{
    public interface IGameView
    {
        Camera Camera { get; }
        IGameAreaView GameAreaView { get; }
        IGameUIView GameUIView { get; }
    }
}