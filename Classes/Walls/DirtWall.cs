namespace CellularAutomaton.Classes.Walls
{
    using System;
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;

    public class DirtWall : IWall
    {
        private static readonly Sprite Sprite = new (Scene.Texture, new IntRect(0, IBlock.Size, IBlock.Size, IBlock.Size));

        public Vector2i Coords { get; set; }

        public int Light { get; set; }

        public int LightDiffusion { get; set; } = 10;

        public RectangleShape CollisionBox { get; set; } = new (new Vector2f(IBlock.Size, IBlock.Size));

        public bool IsVisible { get; set; } = false;

        public IWall Copy()
            => new DirtWall()
            {
                CollisionBox = new RectangleShape(this.CollisionBox),
                Coords = this.Coords,
                Light = this.Light,
            };

        public void Draw(RenderWindow window)
        {
            DirtWall.Sprite.Position = this.CollisionBox.Position;
            window.Draw(DirtWall.Sprite);

            var shadow = new RectangleShape(this.CollisionBox)
            {
                FillColor = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - this.Light))),
            };
            window.Draw(shadow);
        }

        public void Update(Scene scene)
        {
        }
    }
}
