namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public abstract class Block : IGameObject
{
    public static readonly int Size = 20;

    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(60, 20, 20, 20));

    public virtual Sprite Sprite { get => Block.SpriteSource; }

    public virtual int LightDiffusion { get => 50; }

    public virtual bool IsTransparent { get => false; }

    public virtual bool IsCollidable { get => true; }

    public virtual bool IsClimbable { get => false; }

    public virtual bool IsIndestructible { get => false; }

    public virtual RectangleShape CollisionBox { get; set; } = new (new Vector2f(Block.Size, Block.Size));

    public Vector2i Coord { get; set; }

    public bool WasUpdated { get; set; } = false;

    public Chunk Chunk { get; set; }

    public virtual void Draw(RenderTarget target, RenderStates states)
    {
        var light = MathF.Max(0, MathF.Min(1, this.Chunk.LightMesh[this.Coord] / 255f));
        var color = this.Sprite.Color;

        using var sprite = new Sprite(this.Sprite)
        { Color = new ((byte)(color.R * light), (byte)(color.G * light), (byte)(color.B * light)) };
        target.Draw(sprite, states);
    }

    public virtual void OnCreate()
    {
    }

    public virtual void OnUpdate()
    {
        if (this is ITimedEntity timedEntity && timedEntity.IsLifeTimeActive)
        {
            if (this.Chunk.Scene.Clock.ElapsedTime.AsSeconds() >= timedEntity.LifeTimeEnd)
            {
                timedEntity.OnTimeOut();
            }
        }
    }

    public virtual void OnFixedUpdate()
    {
    }

    public virtual void OnCollision(IGameObject gameObject, Vector2f? normal)
    {
    }

    public virtual void OnClick()
    {
    }

    public virtual void OnDestroy()
    {
        this.CollisionBox.Dispose();
    }

    public bool IsBoundary()
    {
        foreach (var delta in Scene.ExpandedNeighborhood)
        {
            var coord = this.Coord + delta;
            var block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
            if (block is Empty || (block is not null && (block.IsTransparent || !block.IsCollidable)))
            {
                return true;
            }
        }

        return false;
    }

    public bool HasNeighbour()
    {
        foreach (var delta in Scene.Neighborhood)
        {
            var coord = this.Coord + delta;
            var block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
            if (block is not Empty)
            {
                return true;
            }
        }

        return false;
    }

    public abstract IGameObject Copy();
}
