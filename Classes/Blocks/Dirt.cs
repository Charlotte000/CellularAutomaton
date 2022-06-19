namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Interfaces;
using SFML.Graphics;

public class Dirt : Block, ICollidable
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(20, 20, 20, 20));

    public override Sprite Sprite { get => Dirt.SpriteSource; }

    public override int LightDiffusion { get => 50; }

    public override bool IsTransparent { get => false; }

    public override Block Copy()
        => new Dirt()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coords = this.Coords,
            Light = this.Light,
            WasUpdated = this.WasUpdated,
        };
}
