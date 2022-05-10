namespace CellularAutomaton.Classes.Blocks
{
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;

    public class Dirt : IBlock, ICollidable
    {
        private static readonly Sprite Sprite = new (Scene.Texture, new IntRect(0, 0, IBlock.Size, IBlock.Size));

        public virtual int LightDiffusion { get; set; } = 50;

        public Vector2i Coords { get; set; }

        public virtual int Light { get; set; }

        public RectangleShape CollisionBox { get; set; } = new RectangleShape(new Vector2f(IBlock.Size, IBlock.Size));

        public IWall Wall { get; set; }

        public bool WasUpdated { get; set; } = false;

        public bool IsVisible { get; set; } = false;

        public virtual void Update(Scene scene)
        {
        }

        public virtual void Draw(RenderWindow window)
        {
            if (this.Light > 0)
            {
                Dirt.Sprite.Position = this.CollisionBox.Position;
                window.Draw(Dirt.Sprite);
            }

            var shadow = new RectangleShape(this.CollisionBox)
            {
                FillColor = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - this.Light))),
            };
            window.Draw(shadow);
        }

        public virtual IBlock Copy()
            => new Dirt()
            {
                CollisionBox = new RectangleShape(this.CollisionBox),
                Coords = this.Coords,
                Light = this.Light,
                Wall = this.Wall.Copy(),
                WasUpdated = this.WasUpdated,
            };
    }
}
