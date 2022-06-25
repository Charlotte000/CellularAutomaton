﻿namespace CellularAutomaton.Classes.Menus;

using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class PauseMenu : Menu
{
    public PauseMenu(RenderWindow window, Vector2f position, Vector2f size)
        : base(window, position, size)
    {
        this.Childs.Add(new Label(window, new Vector2f(size.X / 2, 50), this, "Pause"));

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
}
