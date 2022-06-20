namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Interfaces;
using SFML.Graphics;

public class Torch : Block, ILightSource
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(100, 60, 20, 20));

    public override Sprite Sprite { get => Torch.SpriteSource; }

    public override int LightDiffusion { get => 0; }

    public override bool IsTransparent { get => true; }

    public override bool IsCollidable { get => false; }

    public int Brightness { get; set; } = 300;

    public override Block Copy()
        => new Torch()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coord = this.Coord,
            Light = this.Light,
            WasUpdated = this.WasUpdated,
        };
}
