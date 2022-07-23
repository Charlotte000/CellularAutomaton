namespace CellularAutomaton.Classes.Entities;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Utils;
using CellularAutomaton.Interfaces;
using Newtonsoft.Json;
using SFML.Graphics;
using SFML.System;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public abstract class Entity : IGameObject
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new (60, 20, 20, 20));

    public virtual float PressureIn { get => 0; }

    public virtual float PressureOut { get => 0; }

    public virtual float AirResistance { get => .8f; }

    public virtual float LiquidResistance { get => .75f; }

    public virtual Vector2f LiquidLift { get => new (0, 0); }

    public virtual Vector2f Gravity { get => new (0, .1f); }

    public virtual Sprite Sprite { get => Entity.SpriteSource; }

    [JsonRequired]
    public virtual RectangleShape CollisionBox { get; set; } = new (new Vector2f(20, 20));

    public virtual bool IsCollidable { get => false; }

    public virtual bool IsIndestructible { get => false; }

    public bool IsOnLiquid { get; set; }

    public bool IsOnGround { get; set; }

    [JsonRequired]
    public Vector2f Vel { get; set; }

    public Scene Scene { get; set; }

    public Vector2i Coord
    {
        get => (Vector2i)((this.CollisionBox.Position + (this.CollisionBox.Size / 2)) / Block.Size);
        set => this.CollisionBox.Position = (Vector2f)(value * Block.Size) -
            (this.CollisionBox.Size / 2) + (new Vector2f(Block.Size, Block.Size) / 2);
    }

    public Vector2f Center
    {
        get => this.CollisionBox.Position - this.CollisionBox.Origin + (this.CollisionBox.Size / 2);
        set => this.CollisionBox.Position = value + this.CollisionBox.Origin - (this.CollisionBox.Size / 2);
    }

    public abstract IGameObject Copy();

    public virtual void Draw(RenderTarget target, RenderStates states)
    {
        var coord = this.Coord;
        var light = MathF.Max(0, MathF.Min(1, (this.Scene.ChunkMesh[coord]?.LightMesh[coord] ?? 0) / 255f));
        var color = this.Sprite.Color;

        using var sprite = new Sprite(this.Sprite)
        { Color = new ((byte)(color.R * light), (byte)(color.G * light), (byte)(color.B * light)) };
        target.Draw(sprite, states);
    }

    public virtual void OnCollision(IGameObject gameObject, Vector2f? contactNormal)
    {
        this.IsOnGround |= contactNormal?.Y == -1 && gameObject.IsCollidable;
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
            throwable.Throw(this.Scene.Application.GetMousePosition());
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

        this.Vel += this.Gravity + (this.IsOnLiquid ? this.LiquidLift : new (0, 0));

        var coord = this.Coord;
        if (this.PressureIn != 0)
        {
            this.Vel += this.Scene.ChunkMesh[coord]?.PressureMesh[coord] * this.PressureIn ?? new Vector2f(0, 0);
        }

        this.Vel *= this.IsOnLiquid ? this.LiquidResistance : this.AirResistance;

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

    internal virtual void Collision()
    {
        this.IsOnLiquid = false;
        this.IsOnGround = false;

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
