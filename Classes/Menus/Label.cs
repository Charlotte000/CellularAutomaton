﻿namespace CellularAutomaton.Classes.Menus;

using SFML.Graphics;
using SFML.System;

public class Label : Menu
{
    private readonly Text text;

    public Label(RenderWindow window, Vector2f position, Menu parent, string text, uint fontSize = 30)
        : base(window, position, new Vector2f(0, 0), parent)
    {
        this.Shape.OutlineThickness = 0;

        this.text = new Text(text, Menu.Font, fontSize)
        {
            Position = this.Shape.Position + (this.Shape.Size / 2),
            FillColor = Color.Yellow,
        };

        var bound = this.text.GetLocalBounds();
        this.Shape.Size = new Vector2f(bound.Width, bound.Height);
        this.text.Origin = this.Shape.Size / 2;
    }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        base.Draw(target, states);
        target.Draw(this.text, states);
    }

    public override void OnDelete()
    {
        base.OnDelete();
        this.text.Dispose();
    }
}
