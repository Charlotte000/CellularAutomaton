namespace CellularAutomaton.Classes.Walls;

using SFML.Graphics;

public class EmptyWall : Wall
{
    public override Sprite Sprite { get; }

    public override bool IsIndestructible { get => true; }

    public override void Draw(RenderTarget target, RenderStates states)
    {
    }

    public override EmptyWall Copy()
        => new ()
        {
            Coord = this.Coord,
            CollisionBox = new RectangleShape(this.CollisionBox),
        };
}
