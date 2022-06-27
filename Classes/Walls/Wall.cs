namespace CellularAutomaton.Classes.Walls;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class Wall : IEntity
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(60, 20, 20, 20));

    public virtual Sprite Sprite { get => Wall.SpriteSource; }

    public RectangleShape CollisionBox { get; set; } = new (new Vector2f(Block.Size, Block.Size));

    public Vector2i Coord { get; set; }

    public bool IsCollidable { get => false; }

    public Chunk Chunk { get; set; }

    public virtual IEntity Copy()
        => new Wall()
        {
            Coord = this.Coord,
            CollisionBox = new (this.CollisionBox),
        };

    public virtual void Draw(RenderTarget target, RenderStates states)
    {
        target.Draw(this.Sprite, states);
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

    public virtual void OnCollision(IEntity entity, Vector2f? contactNormal)
    {
    }

    public virtual void OnClick()
    {
    }
}
