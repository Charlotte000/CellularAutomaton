namespace CellularAutomaton.Classes.Blocks;

using SFML.Graphics;

public class Empty : Block
{
    public override int LightDiffusion { get => 15; }

    public override bool IsTransparent { get => true; }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        target.Draw(this.Wall!, states);

        if (this.Wall!.Sprite is not null)
        {
            var shadow = new Sprite(this.Wall.Sprite)
            {
                Color = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - this.Light))),
            };
            target.Draw(shadow, states);
        }
    }

    public override Block Copy()
        => new Empty()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coords = this.Coords,
            Light = this.Light,
            Wall = this.Wall?.Copy(),
            WasUpdated = this.WasUpdated,
        };
}
