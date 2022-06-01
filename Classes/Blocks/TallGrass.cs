namespace CellularAutomaton.Classes.Blocks;

using SFML.Graphics;

public class TallGrass : Block
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(Block.Size * 4, Block.Size, Block.Size, Block.Size));

    public override Sprite Sprite { get => TallGrass.SpriteSource; }

    public override int LightDiffusion { get => 15; }

    public override bool IsTransparent { get => true; }

    public override Block Copy()
        => new TallGrass()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coords = this.Coords,
            Light = this.Light,
            Wall = this.Wall?.Copy(),
            WasUpdated = this.WasUpdated,
        };
}
