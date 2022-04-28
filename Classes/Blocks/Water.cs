namespace CellularAutomaton.Classes.Blocks
{
    using System;
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;

    public class Water : IBlock
    {
        private static Sprite[] sprites = new Sprite[]
        {
            new (Scene.Texture, new IntRect(IBlock.Size, IBlock.Size * 3 / 4, IBlock.Size, IBlock.Size / 4))
            {
                Origin = new Vector2f(0, -IBlock.Size * 3 / 4),
            },
            new (Scene.Texture, new IntRect(IBlock.Size, IBlock.Size / 2, IBlock.Size, IBlock.Size / 2))
            {
                Origin = new Vector2f(0, -IBlock.Size / 2),
            },
            new (Scene.Texture, new IntRect(IBlock.Size, IBlock.Size / 4, IBlock.Size, IBlock.Size * 3 / 4))
            {
                Origin = new Vector2f(0, -IBlock.Size / 4),
            },
            new (Scene.Texture, new IntRect(IBlock.Size, 0, IBlock.Size, IBlock.Size)),
        };

        public byte LightDiffusion { get; set; } = 10;

        public bool IsCollidable { get; set; } = false;

        public Vector2i Coords { get; set; }

        public byte Light { get; set; }

        public RectangleShape CollisionBox { get; set; } = new RectangleShape(new Vector2f(IBlock.Size, IBlock.Size));

        public int Amount { get; set; } = 4;

        public IWall Wall { get; set; }

        public void Update(Scene scene) // HACK: water amount
        {
            // Fall down
            var block = scene.GetBlock(this.Coords.X, this.Coords.Y + 1);
            if (block is not null && block is not Water && !block.IsCollidable)
            {
                scene.SetBlock(this.Copy(), this.Coords.X, this.Coords.Y + 1);
                scene.SetBlock(new Empty(), this.Coords);
                return;
            }

            if (block is Water && (block as Water).Amount < 4)
            {
                var water = block as Water;
                water.Amount += this.Amount;
                this.Amount = 0;
                if (water.Amount > 4)
                {
                    this.Amount = 4 - water.Amount;
                    water.Amount = 4;
                }

                if (this.Amount < 1)
                {
                    scene.SetBlock(new Empty(), this.Coords);
                }

                return;
            }

            // Spread out
            int deltaX = (scene.RandomGenerator.Next(0, 2) * -2) + 1;
            block = scene.GetBlock(this.Coords.X + deltaX, this.Coords.Y);
            if (block is Empty)
            {
                var prevAmount = this.Amount;

                this.Amount /= 2;

                scene.SetBlock(
                    new Water()
                    {
                        Amount = prevAmount - this.Amount,
                        Light = this.Light,
                    },
                    this.Coords.X + deltaX,
                    this.Coords.Y);

                if (this.Amount < 1)
                {
                    scene.SetBlock(new Empty(), this.Coords);
                }

                return;
            }

            if (block is Water && (block as Water).Amount < 4 && (block as Water).Amount < this.Amount)
            {
                var water = block as Water;
                water.Amount++;
                this.Amount--;
                if (this.Amount < 1)
                {
                    scene.SetBlock(new Empty(), this.Coords);
                }

                return;
            }
        }

        public void Draw(RenderWindow window)
        {
            if (this.Amount >= 1 && this.Amount < 5)
            {
                this.Wall.Draw(window);

                Water.sprites[this.Amount - 1].Position = this.CollisionBox.Position;
                window.Draw(Water.sprites[this.Amount - 1]);

                var shadow = new Sprite(Water.sprites[this.Amount - 1])
                {
                    Color = new Color(0, 0, 0, (byte)Math.Max(0, 255 - this.Light)),
                };
                window.Draw(shadow);
            }
        }

        public IBlock Copy()
            => new Water() { Amount = this.Amount, Light = this.Light };
    }
}
