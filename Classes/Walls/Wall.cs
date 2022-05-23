namespace CellularAutomaton.Classes.Walls
{
    using CellularAutomaton.Classes.Blocks;
    using SFML.Graphics;

    public class Wall
    {
        private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(Block.Size * 2, Block.Size, Block.Size, Block.Size));

        public virtual Sprite Sprite { get => Wall.SpriteSource; }

        public virtual Wall Copy()
            => new ();

        public virtual void Draw(RenderWindow window, Block parentBlock)
        {
            this.Sprite.Position = parentBlock.CollisionBox.Position;
            window.Draw(this.Sprite);
        }

        public virtual void Dispose()
        {
        }
    }
}
