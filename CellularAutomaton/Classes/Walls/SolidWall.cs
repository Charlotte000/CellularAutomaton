namespace CellularAutomaton.Classes.Walls
{
    using System;
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;

    public class SolidWall : IWall
    {
        private static Sprite sprite = new (Scene.Texture, new IntRect(0, IBlock.Size, IBlock.Size, IBlock.Size));

        public Vector2i Coords { get; set; }

        public int Light { get; set; }

        public int LightDiffusion { get; set; } = 10;

        public RectangleShape CollisionBox { get; set; } = new (new Vector2f(IBlock.Size, IBlock.Size));

        public IWall Copy()
            => new SolidWall()
            {
                CollisionBox = new RectangleShape(this.CollisionBox),
                Coords = this.Coords,
                Light = this.Light,
            };

        public void Draw(RenderWindow window)
        {
            SolidWall.sprite.Position = this.CollisionBox.Position;
            window.Draw(SolidWall.sprite);

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
