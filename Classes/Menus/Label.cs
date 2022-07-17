namespace CellularAutomaton.Classes.Menus;

using SFML.Graphics;
using SFML.System;

public class Label : Menu
{
    private readonly Text text;

    public Label(Scene scene, Vector2f position, Menu parent, string text, uint fontSize = 30)
        : base(scene, position, new Vector2f(0, 0), parent)
    {
        this.Shape.OutlineThickness = 0;

        this.text = new Text(text, Menu.Font, fontSize)
        {
            Position = this.Shape.Position + (this.Shape.Size / 2),
            FillColor = Color.Yellow,
        };

        var bound = this.text.GetLocalBounds();
        this.text.Origin = new Vector2f((bound.Left * 2) + bound.Width, (bound.Top * 2) + bound.Height) / 2;
    }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        base.Draw(target, states);
        target.Draw(this.text, states);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        this.text.Dispose();
    }
}
