namespace CellularAutomaton.Classes.Blocks;

using SFML.Graphics;
using SFML.System;

public class Empty : Block
{
    public override int LightDiffusion { get => 15; }

    public override bool IsTransparent { get => true; }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        var shadow = new RectangleShape(this.CollisionBox)
        {
            FillColor = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - this.Light))),
            Position = new Vector2f(0, 0),
        };
        target.Draw(shadow, states);
    }

    public override Block Copy()
        => new Empty()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coords = this.Coords,
            Light = this.Light,
            WasUpdated = this.WasUpdated,
        };
}
