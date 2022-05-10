namespace CellularAutomaton.Classes.Walls
{
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;

    public class DirtWall : BaseWall
    {
        private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(0, IBlock.Size, IBlock.Size, IBlock.Size));

        public override Sprite Sprite { get => DirtWall.SpriteSource; }

        public override IWall Copy()
            => new DirtWall();
    }
}
