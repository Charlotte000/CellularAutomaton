namespace CellularAutomaton.Classes.Blocks
{
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;

    public class Torch : BaseBlock, ILightSource
    {
        private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(IBlock.Size, IBlock.Size, IBlock.Size, IBlock.Size));

        public override Sprite Sprite { get => Torch.SpriteSource; }

        public override int LightDiffusion { get => 0; }

        public override bool IsTransparent { get => true; }

        public int Brightness { get; set; } = 300;

        public override IBlock Copy()
            => new Torch()
            {
                CollisionBox = new RectangleShape(this.CollisionBox),
                Coords = this.Coords,
                Light = this.Light,
                Wall = this.Wall.Copy(),
                WasUpdated = this.WasUpdated,
            };
    }
}
