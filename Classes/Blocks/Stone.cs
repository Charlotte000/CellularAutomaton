namespace CellularAutomaton.Classes.Blocks;

using SFML.Graphics;

public class Stone : Block
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(100, 40, 20, 20));

    public override Sprite Sprite { get => Stone.SpriteSource; }

    public override int LightDiffusion { get => 50; }

    public override bool IsTransparent { get => false; }

    public override bool IsCollidable { get => true; }

    public override bool IsClimbable { get => false; }

    public override Block Copy()
        => new Stone()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coords = this.Coords,
            Light = this.Light,
            WasUpdated = this.WasUpdated,
        };
}
