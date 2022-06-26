namespace CellularAutomaton.Classes.Blocks;

using SFML.Graphics;

public class Stone : Block
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(80, 20, 20, 20));

    public override Sprite Sprite { get => Stone.SpriteSource; }

    public override Block Copy()
        => new Stone()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coord = this.Coord,
            WasUpdated = this.WasUpdated,
        };
}
