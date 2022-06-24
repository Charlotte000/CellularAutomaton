namespace CellularAutomaton.Classes.Walls;

using SFML.Graphics;

public class DirtWall : Wall
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(100, 40, 20, 20));

    public override Sprite Sprite { get => DirtWall.SpriteSource; }

    public override Wall Copy()
        => new DirtWall();
}
