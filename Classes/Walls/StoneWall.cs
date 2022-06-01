namespace CellularAutomaton.Classes.Walls;

using CellularAutomaton.Classes.Blocks;
using SFML.Graphics;

public class StoneWall : Wall
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(Block.Size * 3, Block.Size, Block.Size, Block.Size));

    public override Sprite Sprite { get => StoneWall.SpriteSource; }

    public override Wall Copy()
        => new StoneWall();
}
