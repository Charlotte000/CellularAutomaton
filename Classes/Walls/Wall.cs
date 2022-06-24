namespace CellularAutomaton.Classes.Walls;

using SFML.Graphics;
using SFML.System;

public class Wall : Drawable
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(60, 20, 20, 20));

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
