namespace CellularAutomaton.Classes.Walls
{
    using CellularAutomaton.Classes.Blocks;
    using SFML.Graphics;

    public class EmptyWall : Wall
    {
        public override Sprite Sprite { get; }

        public override Wall Copy()
            => new EmptyWall();

        public override void Draw(RenderWindow window, Block parentBlock)
        {
        }
    }
}
