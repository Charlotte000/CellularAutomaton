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

        public RectangleShape CollisionBox { get; set; } = new RectangleShape(new Vector2f(IBlock.Size, IBlock.Size));

        public IWall Wall { get; set; }

        public bool WasUpdated { get; set; } = false;

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
