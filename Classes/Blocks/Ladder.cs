namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Interfaces;
using SFML.Graphics;

public class Ladder : Block, IClimbable
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(Block.Size * 4, 0, Block.Size, Block.Size));

    public override Sprite Sprite { get => Ladder.SpriteSource; }

    public override int LightDiffusion { get => 15; }

    public override bool IsTransparent { get => true; }

    public override void OnCreate(Scene scene)
    {
        this.Expand(scene, 10);
    }

    public override Block Copy()
        => new Ladder()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coords = this.Coords,
            Light = this.Light,
            Wall = this.Wall?.Copy(),
            WasUpdated = this.WasUpdated,
        };

    public void Expand(Scene scene, int length)
    {
        for (int i = 1; i < length; i++)
        {
            if (scene.GetBlock(this.Coords.X, this.Coords.Y + i) is not Empty)
            {
                return;
            }

            scene.SetBlock(new Ladder(), this.Coords.X, this.Coords.Y + i, false, true);
        }
    }
}
