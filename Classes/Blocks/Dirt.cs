namespace CellularAutomaton.Classes.Blocks;

using SFML.Graphics;

public class Dirt : Block
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(20, 20, 20, 20));

    public override Sprite Sprite { get => Dirt.SpriteSource; }

    public override Dirt Copy()
        => new ()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coord = this.Coord,
            WasUpdated = this.WasUpdated,
        };
}
