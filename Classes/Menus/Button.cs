namespace CellularAutomaton.Classes.Menus;

using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Button : Menu
{
    private readonly Sprite? sprite = null;

    private readonly Action onClicked;

    public Button(RenderWindow window, Vector2f position, Vector2f size, Menu parent, Action onClicked)
    : base(window, position, size, parent)
    {
        this.onClicked = onClicked;
    }

    public Button(RenderWindow window, Vector2f position, Vector2f size, Sprite sprite, Menu parent, Action onClicked)
        : base(window, position, size, parent)
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

    public Button(RenderWindow window, Vector2f position, Vector2f size, string str, Menu parent, Action onClicked)
        : this(window, position, size, Button.GetTextSprite(str), parent, onClicked)
    {
    }

    public override void AddEvents()
    {
        this.Window.MouseButtonPressed += this.OnMouseButtonPressed;
        this.Window.MouseMoved += this.OnMouseMoved;
    }

    public override void DeleteEvents()
    {
        this.Window.MouseButtonPressed -= this.OnMouseButtonPressed;
        this.Window.MouseMoved -= this.OnMouseMoved;
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

    public override void OnDelete()
    {
        base.OnDelete();
        this.sprite?.Dispose();
    }

    private static Sprite GetTextSprite(string str)
    {
        var text = new Text(str, Menu.Font, 25) { FillColor = Color.Yellow };

        var textBound = text.GetGlobalBounds();
        var renderTexture = new RenderTexture(
            (uint)((textBound.Left * 2) + textBound.Width),
            (uint)((textBound.Top * 2) + textBound.Height));
        renderTexture.Draw(text);
        renderTexture.Display();

        return new (new Texture(renderTexture.Texture));
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
