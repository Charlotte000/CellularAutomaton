namespace CellularAutomaton.Classes.Menus;

using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Input : Menu
{
    private readonly Label value;

    public Input(
        Application application,
        Vector2f position,
        Vector2f size,
        Menu parent,
        string placeholder = "",
        string defaultValue = "")
        : base(application, position, size, parent)
    {
        this.value = new Label(application, new (size.X / 2, size.Y / 6 * 4), this, defaultValue);
        this.Childs.Add(this.value);
        this.Childs.Add(new Label(application, new (size.X / 2, size.Y / 6), this, placeholder, 15));
    }

    public bool IsFocus { get; set; } = false;

    public string Value { get => this.value.Text; }

    public override bool IsActive
    {
        get => base.IsActive;
        set
        {
            base.IsActive = value;
            if (!value)
            {
                this.IsFocus = false;
            }
        }
    }

    public override void AddEvents()
    {
        this.Application.Window.MouseButtonPressed += this.OnMouseButtonPressed;
        this.Application.Window.KeyPressed += this.OnKeyPressed;
    }

    public override void DeleteEvents()
    {
        this.Application.Window.MouseButtonPressed -= this.OnMouseButtonPressed;
        this.Application.Window.KeyPressed -= this.OnKeyPressed;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        this.Shape.FillColor = this.IsFocus ? new Color(100, 100, 100) : new Color(50, 50, 50);
    }

    private void OnMouseButtonPressed(object? sender, MouseButtonEventArgs e)
    {
        if (this.IsActive && e.Button == Mouse.Button.Left)
        {
            this.IsFocus = this.Shape.GetGlobalBounds().Contains(e.X, e.Y);
        }
    }

    private void OnKeyPressed(object? sender, KeyEventArgs e)
    {
        if (this.IsActive && this.IsFocus && !e.System)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Backspace:
                    if (this.value.Text.Length > 0)
                    {
                        this.value.Text = this.value.Text[..^1];
                    }

                    break;
                case Keyboard.Key.Enter:
                    this.IsFocus = false;
                    break;
                default:
                    if (e.Code.ToString().Length == 1)
                    {
                        this.value.Text += e.Code.ToString().ToLower();
                    }

                    break;
            }
        }
    }
}
