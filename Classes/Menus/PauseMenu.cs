namespace CellularAutomaton.Classes.Menus;

using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class PauseMenu : Menu
{
    public PauseMenu(Scene scene, Vector2f position, Vector2f size)
        : base(scene, position, size)
    {
        this.Childs.Add(new Label(this.Scene, new Vector2f(size.X / 2, 50), this, "Pause"));

        this.Childs.Add(new Button(
            this.Scene,
            new Vector2f((size.X / 2) - 50, 100),
            new Vector2f(100, 50),
            this,
            () =>
            {
                scene.History.SaveHistory();
                this.IsActive = false;
            },
            "Save"));

        this.Childs.Add(new Button(
            this.Scene,
            new Vector2f((size.X / 2) - 50, 200),
            new Vector2f(100, 50),
            this,
            () => this.Scene.Window.Close(),
            "Exit"));

        this.IsActive = false;
    }

    public override void AddEvents()
    {
        this.Scene.Window.KeyPressed += this.OnKeyPressed;
    }

    public override void DeleteEvents()
    {
        this.Scene.Window.KeyPressed -= this.OnKeyPressed;
    }

    private void OnKeyPressed(object? sender, KeyEventArgs e)
    {
        if (e.Code == Keyboard.Key.Escape)
        {
            this.IsActive = !this.IsActive;
        }
    }
}
