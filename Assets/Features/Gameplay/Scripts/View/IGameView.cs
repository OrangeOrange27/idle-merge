namespace Features.Gameplay.View
{
    public interface IGameView
    {
        IGameAreaView GameAreaView { get; }
        IGameUIView GameUIView { get; }
    }
}