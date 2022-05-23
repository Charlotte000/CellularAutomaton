﻿namespace CellularAutomaton.Classes.Entities
{
    using CellularAutomaton.Classes.Blocks;
    using CellularAutomaton.Classes.Utils;
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;
    using SFML.Window;

    public class Player : IMovingEntity
    {
        public Player(float x, float y)
        {
            this.CollisionBox = new RectangleShape(new Vector2f(Block.Size * .9f, Block.Size * 1.9f))
            {
                Position = new Vector2f(x, y),
                FillColor = new Color(255, 0, 0),
            };
        }

        public Vector2f Vel { get; set; } = new Vector2f(0, 0);

        public bool IsOnGround { get; set; } = false;

        public bool IsOnWater { get; set; } = false;

        public RectangleShape CollisionBox { get; set; }

        public bool IsVisible { get; set; } = false;

        public int Light { get; set; }

        public void Draw(RenderWindow window)
        {
            window.Draw(this.CollisionBox);

            var shadow = new RectangleShape(this.CollisionBox)
            {
                FillColor = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - this.Light))),
            };
            window.Draw(shadow);
        }

        public void Update(Scene scene)
        {
            this.Vel += IMovingEntity.Gravity * (this.IsOnWater ? .2f : 1f);
            this.Control();

            this.Vel *= this.IsOnWater ? .8f : .87f;

            this.Collision(scene);
            this.CollisionBox.Position += this.Vel;

            var block = scene.GetBlock((Vector2i)(this.CollisionBox.Position / Block.Size));
            this.Light = block is not null ? block.Light : this.Light;
        }

        public void OnCollision(IEntity entity, Vector2f normal)
        {
            this.IsOnWater |= entity is Water;
            this.IsOnGround |= normal.Y == -1 && entity is ICollidable;
        }

        private void Control()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                this.Vel += new Vector2f(.7f, 0);
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                this.Vel += new Vector2f(-.7f, 0);
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.W) && this.IsOnGround)
            {
                this.Vel += new Vector2f(0, -15f);
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                this.Vel += new Vector2f(0, 1f);
            }
        }

        private void Collision(Scene scene)
        {
            this.IsOnGround = false;
            this.IsOnWater = false;

            var entities = new List<IEntity>();
            var coord = (this.CollisionBox.Position + (this.CollisionBox.Size / 2)) / Block.Size;
            for (int x = (int)coord.X - 2; x < (int)coord.X + 3; x++)
            {
                for (int y = (int)coord.Y - 3; y < (int)coord.Y + 4; y++)
                {
                    var block = scene.GetBlock(x, y);
                    if (block is not null && block is not Empty)
                    {
                        entities.Add(block);
                    }
                }
            }

            AABBCollision.Collision(scene, this, entities);
        }
    }
}
