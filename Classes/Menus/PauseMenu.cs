namespace CellularAutomaton.Classes.Menus;

using SFML.System;
using SFML.Window;

public class PauseMenu : Menu
{
    private readonly Button save;

    private readonly Input saveName;

    private readonly Button exit;

    public PauseMenu(Application application, Vector2f position, Vector2f size)
        : base(application, position, size)
    {
        this.saveName = new (
            this.Application,
            new Vector2f((size.X / 2) - 100, 200),
            new Vector2f(200, 50),
            this,
            "Save name",
            application.Scene.SaveFile ?? string.Empty);
        this.save = new (
            this.Application,
            new Vector2f((size.X / 2) - 100, 100),
            new Vector2f(200, 50),
            this,
            () =>
            {
                if (string.IsNullOrEmpty(this.saveName!.Value))
                {
                    this.saveName.IsFocus = true;
                }
                else
                {
                    this.Application.Scene.SaveFile = this.saveName.Value;
                    this.Application.Scene.History.SaveHistory();
                    this.IsActive = false;
                }
            },
            "Save");
        this.exit = new (
            this.Application,
            new Vector2f((size.X / 2) - 100, 300),
            new Vector2f(200, 50),
            this,
            () => this.Application.Window.Close(),
            "Exit");

        this.Childs.Add(new Label(this.Application, new Vector2f(size.X / 2, 50), this, "Pause"));
        this.Childs.Add(this.save);
        this.Childs.Add(this.saveName);
        this.Childs.Add(this.exit);

        this.IsActive = false;
    }

    public override void AddEvents()
    {
        this.Application.Window.KeyPressed += this.OnKeyPressed;
    }

    public override void DeleteEvents()
    {
        this.Application.Window.KeyPressed -= this.OnKeyPressed;
    }

    private void OnKeyPressed(object? sender, KeyEventArgs e)
    {
        if (e.Code == Keyboard.Key.Escape)
        {
            this.IsActive = !this.IsActive;
        }
    }
}
