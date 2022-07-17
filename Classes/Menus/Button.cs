namespace CellularAutomaton.Classes.Menus;

using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Button : Menu
{
    private readonly Sprite? sprite = null;

    private readonly Action onClicked;

    public Button(Application application, Vector2f position, Vector2f size, Menu parent, Action onClicked)
    : base(application, position, size, parent)
    {
        this.onClicked = onClicked;
    }

    public Button(Application application, Vector2f position, Vector2f size, Menu parent, Action onClicked, Sprite sprite)
        : base(application, position, size, parent)
    {
        this.sprite = sprite;
        this.sprite.Origin = new Vector2f(0, 0);

        var spriteBound = sprite.GetGlobalBounds();
        var w = spriteBound.Left + spriteBound.Width;
        var h = spriteBound.Top + spriteBound.Height;
        var scale = Math.Min(
            (this.Shape.Size.X - (InventoryMenu.Margin * 2)) / w,
            (this.Shape.Size.Y - (InventoryMenu.Margin * 2)) / h);
        this.sprite.Scale = new Vector2f(scale, scale);
        this.sprite.Origin = new Vector2f(w, h) / 2;
        this.sprite.Position = this.Shape.Position + (this.Shape.Size / 2);

        this.onClicked = onClicked;
    }

    public Button(
        Application application,
        Vector2f position,
        Vector2f size,
        Menu parent,
        Action onClicked,
        string str,
        uint fontSize = 30)
        : this(application, position, size, parent, onClicked)
    {
        this.Childs.Add(new Label(this.Application, size / 2, this, str, fontSize));
    }

    public override void AddEvents()
    {
        this.Application.Window.MouseButtonPressed += this.OnMouseButtonPressed;
        this.Application.Window.MouseMoved += this.OnMouseMoved;
    }

    public override void DeleteEvents()
    {
        this.Application.Window.MouseButtonPressed -= this.OnMouseButtonPressed;
        this.Application.Window.MouseMoved -= this.OnMouseMoved;
    }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        if (this.IsActive)
        {
            base.Draw(target, states);

            if (this.sprite is not null)
            {
                target.Draw(this.sprite, states);
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        this.sprite?.Dispose();
    }

    private void OnMouseButtonPressed(object? sender, MouseButtonEventArgs e)
    {
        if (this.IsActive && e.Button == Mouse.Button.Left)
        {
            if (this.Shape.GetGlobalBounds().Contains(e.X, e.Y))
            {
                this.onClicked();
            }
        }
    }

    private void OnMouseMoved(object? sender, MouseMoveEventArgs e)
    {
        if (this.IsActive)
        {
            this.Shape.FillColor = this.Shape.GetGlobalBounds().Contains(e.X, e.Y) ?
                new Color(100, 100, 100) : new Color(50, 50, 50);
        }
    }
}
