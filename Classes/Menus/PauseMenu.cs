namespace CellularAutomaton.Classes.Menus;

using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class PauseMenu : Menu
{
    private readonly Text PauseText = new ("Pause", Menu.Font, 50);

    public PauseMenu(RenderWindow window, Vector2f position, Vector2f size)
        : base(window, position, size)
    {
        this.PauseText.Position = position + new Vector2f(size.X / 2, 0);

        this.Childs.Add(new Button(
            window,
            new Vector2f((size.X / 2) - 50, 200),
            new Vector2f(100, 50),
            "Exit",
            this,
            () => this.Window.Close()));

        this.IsActive = false;
    }

    public override void AddEvents()
    {
        this.Window.KeyPressed += (s, e) =>
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                this.IsActive = !this.IsActive;
            }
        };
    }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        if (this.IsActive)
        {
            base.Draw(target, states);
            target.Draw(this.PauseText, states);
        }
    }
}
