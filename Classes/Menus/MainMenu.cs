namespace CellularAutomaton.Classes.Menus;

using SFML.Graphics;
using SFML.System;

public class MainMenu : Menu
{
    public MainMenu(Scene scene, Vector2f position, Vector2f size)
        : base(scene, position, size)
    {
        this.Shape.FillColor = Color.Transparent;
        this.Childs.Add(new Label(this.Scene, new (size.X / 2, 50), this, "Cellular Automaton"));

        var index = 0;
        foreach (var saveName in MainMenu.GetSaves().Append(null))
        {
            this.Childs.Add(new Button(
                this.Scene,
                new ((size.X / 2) - 150, (index * 100) + 200),
                new (300, 70),
                this,
                () =>
                {
                    this.IsActive = false;
                    this.Scene.Init(saveName!);
                },
                saveName ?? "Create New World"));
            index++;
        }

        this.Childs.Add(new Button(
            this.Scene,
            new ((size.X / 2) - 100, size.Y - 100),
            new (200, 70),
            this,
            () => this.Scene.Window.Close(),
            "Exit"));
    }

    public static string[] GetSaves()
        => Directory.GetFiles(@"..\..\..\Data\Saves")
        .Where(file => file.EndsWith(".txt"))
        .Select(file => Path.GetFileNameWithoutExtension(file))
        .ToArray();
}
