namespace CellularAutomaton.Classes.Walls
{
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;

    public class EmptyWall : IWall
    {
        public Vector2i Coords { get; set; }

        public byte Light { get; set; } = 255;

        public byte LightDiffusion { get; set; } = 0;

        public RectangleShape CollisionBox { get; set; } = new (new Vector2f(IBlock.Size, IBlock.Size));

        public bool IsCollidable { get; set; } = false;

        public IWall Copy()
            => new EmptyWall();

        public void Draw(RenderWindow window)
        {
        }

        public void Update(Scene scene)
        {
        }
    }
}
