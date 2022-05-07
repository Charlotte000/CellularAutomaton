namespace CellularAutomaton.Classes.Walls
{
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;

    public class EmptyWall : IWall
    {
        public Vector2i Coords { get; set; }

        public int Light { get; set; } = 255;

        public int LightDiffusion { get; set; } = 0;

        public RectangleShape CollisionBox { get; set; } = new (new Vector2f(IBlock.Size, IBlock.Size));

        public bool IsVisible { get; set; } = false;

        public IWall Copy()
            => new EmptyWall()
            {
                CollisionBox = new RectangleShape(this.CollisionBox),
                Coords = this.Coords,
                Light = this.Light,
            };

        public void Draw(RenderWindow window)
        {
        }

        public void Update(Scene scene)
        {
        }
    }
}
