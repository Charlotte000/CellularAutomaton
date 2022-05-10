namespace CellularAutomaton.Classes.Walls
{
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;

    public class EmptyWall : BaseWall
    {
        public override Sprite Sprite { get; }

        public override IWall Copy()
            => new EmptyWall();

        public override void Draw(RenderWindow window, IBlock parentBlock)
        {
        }
    }
}
