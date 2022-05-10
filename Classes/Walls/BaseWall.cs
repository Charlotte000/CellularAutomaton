namespace CellularAutomaton.Classes.Walls
{
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;

    public class BaseWall : IWall
    {
        private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(IBlock.Size * 2, IBlock.Size, IBlock.Size, IBlock.Size));

        public virtual Sprite Sprite { get => BaseWall.SpriteSource; }

        public virtual IWall Copy()
            => new BaseWall();

        public virtual void Draw(RenderWindow window, IBlock parentBlock)
        {
            this.Sprite.Position = parentBlock.CollisionBox.Position;
            window.Draw(this.Sprite);
        }
    }
}
