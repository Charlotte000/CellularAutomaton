namespace CellularAutomaton.Classes.Entities;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Utils;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public abstract class Entity : IGameObject
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new (60, 20, 20, 20));

    public virtual float PressureIn { get => 0; }

    public virtual float PressureOut { get => 0; }

    public virtual float AirResistance { get => .8f; }

    public virtual Vector2f Gravity { get => new (0, .1f); }

    public virtual Sprite Sprite { get => Entity.SpriteSource; }

    public virtual RectangleShape CollisionBox { get; set; } = new (new Vector2f(20, 20));

    public Vector2f Vel { get; set; }

    public Scene Scene { get; set; }

    public Vector2i Coord
    {
        get => (Vector2i)((this.CollisionBox.Position + (this.CollisionBox.Size / 2)) / Block.Size);
        set => this.CollisionBox.Position = (Vector2f)(value * Block.Size) - (this.CollisionBox.Size / 2);
    }

    public virtual bool IsCollidable { get => false; }

    public abstract IGameObject Copy();

    public virtual void Draw(RenderTarget target, RenderStates states)
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

    public virtual void OnClick()
    {
    }

    public virtual void OnCollision(IGameObject entity, Vector2f? contactNormal)
    {
    }

    public virtual void OnCreate()
    {
        if (this is ITimedEntity timedEntity && timedEntity.IsLifeTimeActive)
        {
            timedEntity.LifeTimeStart = this.Scene.Clock.ElapsedTime.AsSeconds();
            timedEntity.LifeTimeEnd = timedEntity.LifeTimeStart + timedEntity.LifeTime;
        }

        if (this is IThrowable throwable)
        {
            throwable.Throw((Vector2f)this.Scene.GetMouseCoords() * Block.Size);
        }
    }

    public virtual void OnDestroy()
    {
        this.CollisionBox.Dispose();
    }

    public virtual void OnFixedUpdate()
    {
    }

    public virtual void OnUpdate()
    {
        if (this is ITimedEntity timed)
        {
            if (this.Scene.Clock.ElapsedTime.AsSeconds() >= timed.LifeTimeEnd)
            {
                timed.OnTimeOut();
                return;
            }
        }

        this.Vel += this.Gravity;

        var coord = this.Coord;
        if (this.PressureIn != 0)
        {
            this.Vel += this.Scene.ChunkMesh[coord]?.PressureMesh[coord] * this.PressureIn ?? new Vector2f(0, 0);
        }

        this.Vel *= this.AirResistance;

        this.Collision();
        this.CollisionBox.Position += this.Vel;

        if (this.PressureOut != 0)
        {
            var chunk = this.Scene.ChunkMesh[coord];
            if (chunk is not null)
            {
                chunk.PressureMesh[coord] += this.Vel * this.PressureOut;
            }
        }
    }

    internal void Collision()
    {
        if (this is ILivingEntity living)
        {
            living.IsOnGround = false;
            living.IsOnWater = false;
            living.IsClimbing = false;
        }

        var entities = new List<IGameObject>();
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
