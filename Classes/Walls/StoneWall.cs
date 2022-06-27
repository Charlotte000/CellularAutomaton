namespace CellularAutomaton.Classes.Walls;

using SFML.Graphics;

public class StoneWall : Wall
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(100, 60, 20, 20));

    public override Sprite Sprite { get => StoneWall.SpriteSource; }

    public override StoneWall Copy()
        => new ()
        {
            Coord = this.Coord,
            CollisionBox = new RectangleShape(this.CollisionBox),
        };
}
