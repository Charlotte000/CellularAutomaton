namespace CellularAutomaton.Classes.Blocks
{
    using System;
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;

    public class Solid : IBlock
    {

        private static Sprite sprite = new (Scene.Texture, new IntRect(0, 0, IBlock.Size, IBlock.Size));

        public byte LightDiffusion { get; set; } = 50;

        public bool IsCollidable { get; set; } = true;

        public Vector2i Coords { get; set; }

        public byte Light { get; set; }

        public RectangleShape CollisionBox { get; set; } = new RectangleShape(new Vector2f(IBlock.Size, IBlock.Size));

        public IWall Wall { get; set; }

        public bool WasUpdated { get; set; } = false;

        public void Update(Scene scene)
        {
        }

        public void Draw(RenderWindow window)
        {
            Solid.sprite.Position = this.CollisionBox.Position;
            window.Draw(Solid.sprite);

            var shadow = new RectangleShape(this.CollisionBox)
            {
                FillColor = new Color(0, 0, 0, (byte)Math.Max(0, 255 - this.Light)),
            };
            window.Draw(shadow);
        }

        public IBlock Copy()
            => new Solid()
            {
                CollisionBox = new RectangleShape(this.CollisionBox),
                Coords = this.Coords,
                Light = this.Light,
                Wall = this.Wall.Copy(),
                WasUpdated = this.WasUpdated,
            };
    }
}
