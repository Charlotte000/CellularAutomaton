namespace CellularAutomaton.Classes.Menu;

using SFML.Graphics;
using SFML.System;

public class Interface : Drawable
{
    public static readonly int Margin = 7;

    internal readonly RectangleShape Shape;

    internal readonly RenderWindow Window;

    public Interface(RenderWindow window, Vector2f position, Vector2f size)
    {
        this.Shape = new (size)
        {
            Position = position,
            FillColor = new Color(50, 50, 50),
            OutlineColor = new Color(100, 100, 100),
            OutlineThickness = 3,
        };

        this.Window = window;

        this.AddEvents();
    }

    public virtual void AddEvents()
    {
    }

    public virtual void DeleteEvents()
    {
    }

    public virtual void Draw(RenderTarget target, RenderStates states)
    {
        target.Draw(this.Shape, states);
    }

    public virtual void OnDelete()
    {
        this.Shape.Dispose();
        this.DeleteEvents();
    }
}
