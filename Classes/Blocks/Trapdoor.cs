namespace CellularAutomaton.Classes.Blocks;

using SFML.Graphics;
using SFML.System;

public class Trapdoor : Block
{
    private static readonly Sprite[] SpriteSource = new Sprite[]
    {
        new (Scene.Texture, new IntRect(80, 40, 20, 20)),
        new (Scene.Texture, new IntRect(80, 60, 20, 20)),
    };

    public override Sprite Sprite { get => Trapdoor.SpriteSource[this.IsOpened ? 1 : 0]; }

    public override int LightDiffusion { get => this.IsOpened ? 15 : 50; }

    public override bool IsTransparent { get => true; }

    public override bool IsCollidable { get => !this.IsOpened; }

    public override bool IsClimbable { get => this.IsOpened; }

    public override RectangleShape CollisionBox { get; set; } = new (new Vector2f(Block.Size, Block.Size / 5));

    public bool IsOpened { get; set; } = true;

    public override void OnClick()
    {
        base.OnClick();

        if (this.IsOpened)
        {
            foreach (var entity in this.Chunk.Scene.Entities)
            {
                if (entity.CollisionBox.GetGlobalBounds().Intersects(this.CollisionBox.GetGlobalBounds()))
                {
                    return;
                }
            }
        }

        this.IsOpened = !this.IsOpened;

        this.Chunk.Scene.BlockHistory.SaveBlock(this.Chunk, this);
    }

    public override Trapdoor Copy()
    => new ()
    {
        CollisionBox = new RectangleShape(this.CollisionBox),
        Coord = this.Coord,
        WasUpdated = this.WasUpdated,
        IsOpened = this.IsOpened,
    };
}