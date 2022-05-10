namespace CellularAutomaton.Classes.Blocks
{
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;

    public class Torch : IBlock, ILightSource
    {
        private static readonly Sprite Sprite = new (Scene.Texture, new IntRect(IBlock.Size, IBlock.Size, IBlock.Size, IBlock.Size));

        public Vector2i Coords { get; set; }

        public int Brightness { get; set; } = 300;

        public int Light { get; set; }

        public int LightDiffusion { get; set; } = 0;

        public IWall Wall { get; set; }

        public bool WasUpdated { get; set; }

        public RectangleShape CollisionBox { get; set; } = new (new Vector2f(IBlock.Size, IBlock.Size));

        public bool IsVisible { get; set; } = false;

        public IBlock Copy()
            => new Torch()
            {
                CollisionBox = new RectangleShape(this.CollisionBox),
                Coords = this.Coords,
                Light = this.Light,
                Wall = this.Wall.Copy(),
                WasUpdated = this.WasUpdated,
            };

        public void Draw(RenderWindow window)
        {
            this.Wall.Draw(window);

            Torch.Sprite.Position = this.CollisionBox.Position;
            window.Draw(Torch.Sprite);

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
