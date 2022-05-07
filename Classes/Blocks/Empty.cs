namespace CellularAutomaton.Classes.Blocks
{
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;

    public class Empty : IBlock
    {
        public Vector2i Coords { get; set; }

        public int Light { get; set; } = 255;

        public int LightDiffusion { get; set; } = 25;

        public RectangleShape CollisionBox { get; set; } = new RectangleShape(new Vector2f(IBlock.Size, IBlock.Size));

        public IWall Wall { get; set; }

        public bool WasUpdated { get; set; } = false;

        public bool IsVisible { get; set; } = false;

        public void Update(Scene scene)
        {
        }

        public void Draw(RenderWindow window)
            => this.Wall.Draw(window);

        public IBlock Copy()
            => new Empty()
            {
                CollisionBox = new RectangleShape(this.CollisionBox),
                Coords = this.Coords,
                Light = this.Light,
                Wall = this.Wall.Copy(),
                WasUpdated = this.WasUpdated,
            };
    }
}
