namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Interfaces;
using SFML.Graphics;

public class Dirt : Block, ICollidable
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(0, 0, Block.Size, Block.Size));

    public override Sprite Sprite { get => Dirt.SpriteSource; }

    public override int LightDiffusion { get => 50; }

    public override bool IsTransparent { get => false; }

    public override Block Copy()
        => new Dirt()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coords = this.Coords,
            Light = this.Light,
            Wall = this.Wall?.Copy(),
            WasUpdated = this.WasUpdated,
        };
}
