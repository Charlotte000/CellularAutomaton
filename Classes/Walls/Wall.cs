namespace CellularAutomaton.Classes.Walls;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public abstract class Wall : IGameObject
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(60, 20, 20, 20));

    public virtual Sprite Sprite { get => Wall.SpriteSource; }

    public virtual bool IsIndestructible { get => false; }

    public virtual RectangleShape CollisionBox { get; set; } = new (new Vector2f(Block.Size, Block.Size));

    public Vector2i Coord { get; set; }

    public bool IsCollidable { get => false; }

    public Chunk Chunk { get; set; }

    public abstract IGameObject Copy();

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
    }

    public virtual void OnFixedUpdate()
    {
    }

    public virtual void OnDestroy()
    {
        this.CollisionBox.Dispose();
    }

    public virtual void OnCollision(IGameObject gameObject, Vector2f? contactNormal)
    {
    }

    public virtual void OnClick()
    {
    }
}
