namespace CellularAutomaton.Interfaces
{
    using SFML.Graphics;

    public interface IWall
    {
        public Sprite Sprite { get; }

        public IWall Copy();

        public void Draw(RenderWindow window, IBlock parentBlock);

        public void Dispose()
        {
        }
    }
}
