namespace CellularAutomaton.Classes.Entities;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Utils;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class Leaf : IMovingEntity
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new (320, 84, 5, 5))
    { Origin = new (2.5f, 2.5f) };

    private static Vector2f gravity = new (0, .1f);

    public Leaf(Vector2f position)
    {
        this.CollisionBox.Position = position;
    }

    public Sprite Sprite
    {
        get
        {
            Leaf.SpriteSource.Rotation = this.Angle;
            return Leaf.SpriteSource;
        }
    }

    public float AngleVel { get; set; } = (float)(Scene.RandomGenerator.NextDouble() * 10) - 5;

    public float Angle { get; set; } = (float)(Scene.RandomGenerator.NextDouble() * 360);

    public Vector2f Vel { get; set; }

    public RectangleShape CollisionBox { get; set; } = new (new Vector2f(3, 3));

    public Vector2i Coord
    {
        get => (Vector2i)((this.CollisionBox.Position + (this.CollisionBox.Size / 2)) / Block.Size);
        set => this.CollisionBox.Position = (Vector2f)(value * Block.Size) - (this.CollisionBox.Size / 2);
    }

    public bool IsCollidable { get => false; }

    public Scene Scene { get; set; }

    public void Draw(RenderTarget target, RenderStates states)
    {
        target.Draw(this.Sprite, states);

        var coord = this.Coord;
        var light = this.Scene.ChunkMesh[coord]?.LightMesh[coord] ?? 0;
        var shadow = new Sprite(this.Sprite)
        {
            Color = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - light))),
        };
        target.Draw(shadow, states);
    }

    public void OnCreate()
    {
    }

    public void OnUpdate()
    {
        var coord = this.Coord;

        this.Vel += Leaf.gravity;
        this.Vel += new Vector2f(
            (float)((Scene.RandomGenerator.NextDouble() * .5) - .25),
            (float)((Scene.RandomGenerator.NextDouble() * .5) - .25));
        this.Vel *= .7f;
        this.Vel += this.Scene.ChunkMesh[coord]?.PressureMesh[coord] ?? new Vector2f(0, 0);

        this.Angle += this.AngleVel;

        this.Collision();
        this.CollisionBox.Position += this.Vel;

        var block = this.Scene.ChunkMesh[coord]?.BlockMesh[coord];
        if (block is null)
        {
            this.Vel *= 0;
        }

        if (this.Vel.X == 0 && this.Vel.Y == 0)
        {
            this.Scene.RemoveEntity(this);
            return;
        }
    }

    public void OnFixedUpdate()
    {
    }

    public void OnDestroy()
    {
    }

    public void OnCollision(IEntity entity, Vector2f? contactNormal)
    {
        if (entity.IsCollidable)
        {
            this.Vel *= 0;
        }
    }

    public void OnClick()
    {
    }

    public void OnDelete()
    {
        this.CollisionBox.Dispose();
    }

    public IEntity Copy()
        => new Leaf(this.CollisionBox.Position);

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
