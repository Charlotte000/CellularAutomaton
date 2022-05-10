namespace CellularAutomaton.Classes.Walls
{
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;

    public class StoneWall : BaseWall
    {
        private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(IBlock.Size * 3, IBlock.Size, IBlock.Size, IBlock.Size));

        public override Sprite Sprite { get => StoneWall.SpriteSource; }

        public override IWall Copy()
            => new StoneWall();
    }
}
