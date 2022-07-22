namespace CellularAutomaton.Classes.Menus;

using SFML.Graphics;
using SFML.System;

public class MainMenu : Menu
{
    public MainMenu(Application application, Vector2f position, Vector2f size)
        : base(application, position, size)
    {
        this.Shape.FillColor = Color.Transparent;
        this.Childs.Add(new Label(this.Application, new (size.X / 2, 50), this, "Cellular Automaton"));

        var index = 0;
        foreach (var saveName in MainMenu.GetSaves().Append(null))
        {
            this.Childs.Add(new Button(
                this.Application,
                new ((size.X / 2) - 150, (index * 100) + 200),
                new (300, 70),
                this,
                () =>
                {
                    this.IsActive = false;
                    this.Application.Scene = new Scene(this.Application)
                    { SaveFile = saveName! };
                    this.Application.Scene.OnCreate();
                },
                saveName ?? "Create New World"));
            index++;
        }

        this.Childs.Add(new Button(
            this.Application,
            new ((size.X / 2) - 100, size.Y - 100),
            new (200, 70),
            this,
            () => this.Application.Window.Close(),
            "Exit"));
    }

    public static string[] GetSaves()
        => Directory.GetFiles(@"..\..\..\Data\Saves")
        .Where(file => file.EndsWith(".txt"))
        .Select(file => Path.GetFileNameWithoutExtension(file))
        .ToArray();
}
