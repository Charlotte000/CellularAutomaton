namespace CellularAutomaton.Classes.Entities;

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

    public Scene Scene { get; set; }

    public void Draw(RenderTarget target, RenderStates states)
    {
        target.Draw(this.CollisionBox, states);

        var shadow = new RectangleShape(this.CollisionBox)
        {
            FillColor = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - this.Light))),
        };
        target.Draw(shadow, states);
    }

    public void OnCollision(IEntity entity, Vector2f? contactNormal)
    {
        if (entity is ICollidable)
        {
            this.Vel *= 0;
        }
    }

    public void OnCreate()
    {
    }

    public void OnDelete()
    {
        this.CollisionBox.Dispose();
    }

    public void OnUpdate()
    {
        var coord = (Vector2i)(this.CollisionBox.Position / Block.Size);

        this.Vel += Leaf.gravity;
        this.Vel += new Vector2f(
            (float)((Scene.RandomGenerator.NextDouble() * .5) - .25),
            (float)((Scene.RandomGenerator.NextDouble() * .5) - .25));
        this.Vel *= .7f;
        this.Vel += this.Scene.ChunkMesh[coord]?.PressureMesh[coord] ?? new Vector2f(0, 0);

        this.Collision();
        this.CollisionBox.Position += this.Vel;

        var block = this.Scene.ChunkMesh[coord]?.BlockMesh[coord];
        this.Light = block is not null ? block.Light : this.Light;

        if (block is null)
        {
            this.Vel *= 0;
        }

        if (this.Vel.X == 0 && this.Vel.Y == 0)
        {
            this.Scene.Entities.Remove(this);
            return;
        }
    }

    private void Collision()
    {
        var entities = new List<IEntity>();
        var coord = (this.CollisionBox.Position + (this.CollisionBox.Size / 2)) / Block.Size;

        for (int x = (int)coord.X - 2; x < (int)coord.X + 3; x++)
        {
            for (int y = (int)coord.Y - 3; y < (int)coord.Y + 4; y++)
            {
                var block = this.Scene.ChunkMesh[x, y]?.BlockMesh[x, y];
                if (block is not null && block is not Empty)
                {
                    entities.Add(block);
                }
            }
        }

        AABBCollision.Collision(this.Scene, this, entities);
    }
}
