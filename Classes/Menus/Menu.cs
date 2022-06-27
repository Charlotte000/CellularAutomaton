namespace CellularAutomaton.Classes.Menus;

using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public abstract class Menu : IMonoBehaviour, Drawable
{
    public static readonly int Margin = 7;

    public static readonly Font Font = new (@"..\..\..\Data\arial.ttf");

    internal readonly RectangleShape Shape;

    internal readonly RenderWindow Window;

    internal readonly List<Menu> Childs = new ();

    internal readonly Menu? Parent;

    private bool isActive = true;

    public Menu(RenderWindow window, Vector2f position, Vector2f size, Menu? parent = null)
    {
        this.Shape = new (size)
        {
            Position = position + (parent?.Shape.Position ?? new Vector2f(0, 0)),
            FillColor = new Color(50, 50, 50),
            OutlineColor = new Color(100, 100, 100),
            OutlineThickness = 3,
        };

        this.Parent = parent;
        this.Window = window;

        this.AddEvents();
    }

    public bool IsActive
    {
        get => this.isActive;
        set
        {
            this.isActive = value;
            foreach (var child in this.Childs)
            {
                child.isActive = value;
            }
        }
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

            foreach (var child in this.Childs)
            {
                target.Draw(child, states);
            }
        }
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
        foreach (var child in this.Childs)
        {
            child.OnDestroy();
        }

        this.DeleteEvents();
    }
}
