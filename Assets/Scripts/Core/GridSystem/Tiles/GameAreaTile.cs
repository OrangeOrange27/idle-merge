namespace Core.GridSystem.Tiles
{
    public class GameAreaTile : TileBase, IGameAreaTile
    {
        public bool IsOccupied { get; } = false;
    }
}