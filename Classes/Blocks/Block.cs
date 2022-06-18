namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Classes.Walls;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class Block : IEntity
{
    public static readonly int Size = 20;

    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(Block.Size * 2, Block.Size, Block.Size, Block.Size));

    public virtual Sprite Sprite { get => Block.SpriteSource; }

    public virtual int LightDiffusion { get => 50; }

    public virtual bool IsTransparent { get => false; }

    public virtual RectangleShape CollisionBox { get; set; } = new RectangleShape(new Vector2f(Block.Size, Block.Size));

    public Vector2i Coords { get; set; }

    public int Light { get; set; }

    public bool WasUpdated { get; set; } = false;

    public bool IsVisible { get; set; } = false;

    public Chunk Chunk { get; set; }

    public virtual void OnCreate()
    {
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void Draw(RenderTarget target, RenderStates states)
    {
        if (this.Light > 0)
        {
            target.Draw(this.Sprite, states);
        }

        Drawable shadow = this.Chunk.WallMesh[this.Coords] is not EmptyWall && this.IsTransparent ?
            new RectangleShape(this.CollisionBox)
            {
                FillColor = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - this.Light))),
                Position = new Vector2f(0, 0),
            }
            :
            new Sprite(this.Sprite)
            {
                Color = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - this.Light))),
            };

        target.Draw(shadow, states);
    }

    public virtual void OnCollision(IEntity entity, Vector2f? normal)
    {
    }

    public virtual void OnDelete()
    {
        this.CollisionBox.Dispose();
    }

    public virtual Block Copy()
        => new ()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coords = this.Coords,
            Light = this.Light,
            WasUpdated = this.WasUpdated,
        };
}
