namespace CellularAutomaton.Classes.Blocks
{
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;

    public class Liana : Block, IClimbable
    {
        private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(Block.Size * 5, 0, Block.Size, Block.Size));

        public override Sprite Sprite { get => Liana.SpriteSource; }

        public override int LightDiffusion { get => 15; }

        public override bool IsTransparent { get => true; }

        public override void OnCreate(Scene scene)
            => this.Expand(scene, Scene.RandomGenerator.Next(3, 10));

        public override Block Copy()
            => new Liana()
            {
                CollisionBox = new RectangleShape(this.CollisionBox),
                Coords = this.Coords,
                Light = this.Light,
                Wall = this.Wall.Copy(),
                WasUpdated = this.WasUpdated,
            };

        public void Expand(Scene scene, int length)
        {
            for (int i = 1; i < length; i++)
            {
                var block = scene.GetBlock(this.Coords.X, this.Coords.Y + i);
                if (block is not Empty)
                {
                    return;
                }

                scene.SetBlock(this.Copy(), this.Coords.X, this.Coords.Y + i, false, true);
            }
        }
    }
}
