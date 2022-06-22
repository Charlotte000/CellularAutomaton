namespace CellularAutomaton.Classes.Menus;

using SFML.Graphics;
using SFML.System;

public abstract class Menu : Drawable
{
    public static readonly int Margin = 7;

    public static readonly Font Font = new (@"..\..\..\Data\arial.ttf");

    internal readonly RectangleShape Shape;

    internal readonly RenderWindow Window;

    internal bool IsActive = true;

    public Menu(RenderWindow window, Vector2f position, Vector2f size)
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
        if (this.IsActive)
        {
            target.Draw(this.Shape, states);
        }
    }

    public virtual void OnDelete()
    {
        this.Shape.Dispose();
        this.DeleteEvents();
    }
}
