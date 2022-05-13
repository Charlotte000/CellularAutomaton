namespace CellularAutomaton.Classes.Blocks
{
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;

    public class Grass : BaseBlock, ICollidable
    {
        private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(IBlock.Size * 2, 0, IBlock.Size, IBlock.Size));

        public override Sprite Sprite { get => Grass.SpriteSource; }

        public override int LightDiffusion { get => 50; }

        public override bool IsTransparent { get => false; }

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
