namespace CellularAutomaton.Classes.Blocks
{
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;

    public class Stone : Block, ICollidable
    {
        private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(Block.Size * 3, 0, Block.Size, Block.Size));

        public override Sprite Sprite { get => Stone.SpriteSource; }

        public override int LightDiffusion { get => 50; }

        public override bool IsTransparent { get => false; }


        public override Block Copy()
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
