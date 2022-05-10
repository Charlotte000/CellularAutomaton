namespace CellularAutomaton.Classes.Blocks
{
    using System;
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;

    public class Water : IBlock
    {
        private static readonly Sprite[] Sprites = new Sprite[]
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

        public int LightDiffusion { get; set; } = 10;

        public Vector2i Coords { get; set; }

        public int Light { get; set; }

        public RectangleShape CollisionBox { get; set; } = new RectangleShape(new Vector2f(IBlock.Size, IBlock.Size));

        public IWall Wall { get; set; }

        public int Amount { get; set; } = 4;

        public bool WasUpdated { get; set; } = false;

        public bool IsVisible { get; set; } = false;

        public void Update(Scene scene)
        {
            // Fall down
            if (this.FallDown(scene))
            {
                return;
            }

            // Spread Out
            var deltaX = (scene.RandomGenerator.Next(0, 2) * -2) + 1;
            if (this.SpreadOut(scene, deltaX))
            {
                return;
            }
        }

        public void Draw(RenderWindow window)
        {
            if (this.Amount >= 1 && this.Amount < 5)
            {
                this.Wall.Draw(window);

                Water.Sprites[this.Amount - 1].Position = this.CollisionBox.Position;
                window.Draw(Water.Sprites[this.Amount - 1]);

                var shadow = new Sprite(Water.Sprites[this.Amount - 1])
                {
                    Color = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - this.Light))),
                };
                window.Draw(shadow);
            }
        }

        public IBlock Copy()
            => new Water()
            {
                CollisionBox = new RectangleShape(this.CollisionBox),
                Coords = this.Coords,
                Light = this.Light,
                Amount = this.Amount,
                WasUpdated = this.WasUpdated,
            };

        private bool FallDown(Scene scene)
        {
            var block = scene.GetBlock(this.Coords.X, this.Coords.Y + 1);
            if (block is not null && block is not Water && block is not ICollidable)
            {
                scene.SetBlock(this.Copy(), this.Coords.X, this.Coords.Y + 1, false);
                scene.SetBlock(new Empty() { WasUpdated = true }, this.Coords, false);
                return true;
            }

            if (block is Water water && water.Amount < 4)
            {
                water.WasUpdated = true;
                water.Amount += this.Amount;
                this.Amount = 0;
                if (water.Amount > 4)
                {
                    this.Amount = water.Amount - 4;
                    water.Amount = 4;
                }

                if (this.Amount < 1)
                {
                    scene.SetBlock(new Empty() { WasUpdated = true }, this.Coords, false);
                }

                return true;
            }

            return false;
        }

        private bool SpreadOut(Scene scene, int deltaX)
        {
            var block = scene.GetBlock(this.Coords.X + deltaX, this.Coords.Y);
            if (block is Empty)
            {
                var prevAmount = this.Amount;

                this.Amount /= 2;

                scene.SetBlock(
                    new Water()
                    {
                        Amount = prevAmount - this.Amount,
                        Light = this.Light,
                        WasUpdated = true,
                    },
                    this.Coords.X + deltaX,
                    this.Coords.Y,
                    false);

                if (this.Amount < 1)
                {
                    scene.SetBlock(new Empty() { WasUpdated = true }, this.Coords, false);
                }

                return true;
            }

            if (block is Water water && water.Amount < 4 && water.Amount < this.Amount)
            {
                water.WasUpdated = true;
                water.Amount++;
                this.Amount--;
                if (this.Amount < 1)
                {
                    scene.SetBlock(new Empty() { WasUpdated = true }, this.Coords, false);
                }

                return true;
            }

            return false;
        }
    }
}
