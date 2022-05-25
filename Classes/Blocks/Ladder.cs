namespace CellularAutomaton.Classes.Blocks
{
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;

    public class Ladder : Block
    {
        private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(Block.Size * 4, 0, Block.Size, Block.Size));

        public override Sprite Sprite { get => Ladder.SpriteSource; }

        public override bool IsTransparent { get => true; }

        public override int LightDiffusion { get => 15; }

        public override void OnCollision(IEntity entity, Vector2f? normal)
        {
            if (entity is IMovingEntity movingEntity)
            {
                movingEntity.IsOnLadder = true;
            }
        }

        public override Block Copy()
            => new Ladder()
            {
                CollisionBox = new RectangleShape(this.CollisionBox),
                Coords = this.Coords,
                Light = this.Light,
                Wall = this.Wall.Copy(),
                WasUpdated = this.WasUpdated,
            };
    }
}
