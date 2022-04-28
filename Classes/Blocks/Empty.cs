namespace CellularAutomaton.Classes.Blocks
{
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;

    public class Empty : IBlock
    {
        public Vector2i Coords { get; set; }

        public byte Light { get; set; } = 255;

        public byte LightDiffusion { get; set; } = 25;

        public bool IsCollidable { get; set; } = false;

        public RectangleShape CollisionBox { get; set; } = new RectangleShape(new Vector2f(0, 0));

        public IWall Wall { get; set; }

        public void Update(Scene scene)
        {
        }

        public void Draw(RenderWindow window)
            => this.Wall.Draw(window);

        public IBlock Copy()
            => new Empty();
    }
}
