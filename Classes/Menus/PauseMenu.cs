namespace CellularAutomaton.Classes.Menus;

using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class PauseMenu : Menu
{
    private readonly Button[] buttons;

    public PauseMenu(RenderWindow window, Vector2f position, Vector2f size)
        : base(window, position, size)
    {
        this.IsActive = false;

        this.buttons = new Button[]
        {
            new (window, position, new Vector2f(100, 50), "Close", () =>
            {
                this.IsActive = false;
                foreach (var button in this.buttons!)
                {
                    button.IsActive = this.IsActive;
                }
            }),
            new (window, new Vector2f(200, 100), new Vector2f(100, 100), "Exit", () =>
            {
                this.Window.Close();
            }),
        };
    }

    public override void AddEvents()
    {
        this.Window.KeyPressed += (s, e) =>
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                this.IsActive = !this.IsActive;

                foreach (var button in this.buttons)
                {
                    button.IsActive = this.IsActive;
                }
            }
        };
    }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        if (this.IsActive)
        {
            base.Draw(target, states);

            foreach (var button in this.buttons)
            {
                target.Draw(button, states);
            }
        }
    }
}
