namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Classes.Walls;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class Block : IEntity
{
    public static readonly int Size = 20;

    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(60, 20, 20, 20));

    public virtual Sprite Sprite { get => Block.SpriteSource; }

    public virtual int LightDiffusion { get => 50; }

    public virtual bool IsTransparent { get => false; }

    public virtual bool IsCollidable { get => true; }

    public virtual bool IsClimbable { get => false; }

    public virtual RectangleShape CollisionBox { get; set; } = new (new Vector2f(Block.Size, Block.Size));

    public Vector2i Coord { get; set; }

    public bool WasUpdated { get; set; } = false;

    public Chunk Chunk { get; set; }

    public virtual void Draw(RenderTarget target, RenderStates states)
    {
        var light = this.Chunk.LightMesh[this.Coord];
        if (light > 0)
        {
            target.Draw(this.Sprite, states);
        }

        Drawable shadow = this.Chunk.WallMesh[this.Coord] is not EmptyWall && this.IsTransparent ?
            new RectangleShape(this.CollisionBox)
            {
                FillColor = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - light))),
                Position = new Vector2f(0, 0),
            }
            :
            new Sprite(this.Sprite)
            {
                Color = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - light))),
            };

        target.Draw(shadow, states);
    }

    public virtual void OnCreate()
    {
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnFixedUpdate()
    {
    }

    public virtual void OnCollision(IEntity entity, Vector2f? normal)
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

    public virtual IEntity Copy()
        => new Block()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coord = this.Coord,
            WasUpdated = this.WasUpdated,
        };
}
