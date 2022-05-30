namespace CellularAutomaton.Classes.Entities
{
    using CellularAutomaton.Classes.Blocks;
    using CellularAutomaton.Classes.Utils;
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;

    public class Leaf : IMovingEntity
    {
        private static Vector2f gravity = new (0, .1f);

        public Leaf(Vector2f position)
        {
            this.CollisionBox.Position = position;
        }

        public Vector2f Vel { get; set; }

        public RectangleShape CollisionBox { get; set; } = new (new Vector2f(3, 3))
        {
            FillColor = Color.Green,
        };

        public bool IsVisible { get; set; }

        public int Light { get; set; }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(this.CollisionBox, states);
        }

        public void OnCollision(IEntity entity, Vector2f? contactNormal)
        {
            if (entity is ICollidable)
            {
                this.Vel *= 0;
            }
        }

        public void OnCreate(Scene scene)
        {
        }

        public void OnDelete()
        {
            this.CollisionBox.Dispose();
        }

        public void OnUpdate(Scene scene)
        {
            this.Vel += Leaf.gravity;
            this.Vel += new Vector2f(
                (float)((Scene.RandomGenerator.NextDouble() * .5) - .25),
                (float)((Scene.RandomGenerator.NextDouble() * .5) - .25));
            this.Vel *= .7f;
            this.Vel += scene.GetVel((Vector2i)(this.CollisionBox.Position / Block.Size)) ?? new Vector2f(0, 0);

            this.Collision(scene);
            this.CollisionBox.Position += this.Vel;

            if (this.Vel.X == 0 && this.Vel.Y == 0)
            {
                scene.RemoveEntity(this);
                return;
            }
        }

        private void Collision(Scene scene)
        {
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
