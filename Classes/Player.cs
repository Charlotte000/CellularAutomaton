namespace CellularAutomaton.Classes
{
    using CellularAutomaton.Classes.Blocks;
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;
    using SFML.Window;

    public class Player : IMovingEntity
    {
        public Player(float x, float y)
        {
            this.CollisionBox = new RectangleShape(new Vector2f(IBlock.Size * .9f, IBlock.Size * 1.9f))
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

        public void Draw(RenderWindow window)
        {
            window.Draw(this.CollisionBox);
        }

        public void Update(Scene scene)
        {
            this.Vel += IMovingEntity.Gravity * (this.IsOnWater ? .2f : 1f);
            this.Control();

            this.Vel *= this.IsOnWater ? .8f : .87f;

            this.Collision(scene);
            this.CollisionBox.Position += this.Vel;
        }

        public void OnCollision(IEntity entity, Vector2f normal)
        {
            this.IsOnWater |= entity is Water;
            this.IsOnGround |= normal.Y == -1 && entity is ICollidable;
        }

        public void Control()
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
            var coord = (this.CollisionBox.Position + (this.CollisionBox.Size / 2)) / IBlock.Size;
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
