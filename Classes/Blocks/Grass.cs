namespace CellularAutomaton.Classes.Blocks
{
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;

    internal class Grass : Dirt
    {
        private static readonly Sprite Sprite = new (Scene.Texture, new IntRect(IBlock.Size * 2, 0, IBlock.Size, IBlock.Size));

        public override void Draw(RenderWindow window)
        {
            if (this.Light > 0)
            {
                Grass.Sprite.Position = this.CollisionBox.Position;
                window.Draw(Grass.Sprite);
            }

            var shadow = new RectangleShape(this.CollisionBox)
            {
                FillColor = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - this.Light))),
            };
            window.Draw(shadow);
        }

        public override IBlock Copy()
            => new Grass()
            {
                CollisionBox = new RectangleShape(this.CollisionBox),
                Coords = this.Coords,
                Light = this.Light,
                Wall = this.Wall.Copy(),
                WasUpdated = this.WasUpdated,
            };
    }
}
