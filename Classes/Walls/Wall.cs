namespace CellularAutomaton.Classes.Walls;

using CellularAutomaton.Classes.Blocks;
using SFML.Graphics;
using SFML.System;

public class Wall : Drawable
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(Block.Size * 2, Block.Size, Block.Size, Block.Size));

    public virtual Sprite Sprite { get => Wall.SpriteSource; }

    public Vector2i Coords { get; set; }

    public virtual Wall Copy()
        => new ();

    public virtual void Draw(RenderTarget target, RenderStates states)
    {
        target.Draw(this.Sprite, states);
    }

    public virtual void OnDelete()
    {
    }
}
