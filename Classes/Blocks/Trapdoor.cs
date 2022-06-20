namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Classes.Walls;
using SFML.Graphics;
using SFML.System;

public class Trapdoor : Block
{
    private static readonly Sprite[] SpriteSource = new Sprite[]
    {
        new (Scene.Texture, new IntRect(180, 0, 20, 20)),
        new (Scene.Texture, new IntRect(180, 20, 20, 20)),
    };

    public override Sprite Sprite { get => Trapdoor.SpriteSource[this.IsOpened ? 1 : 0]; }

    public override int LightDiffusion { get => this.IsOpened ? 15 : 50; }

    public override bool IsTransparent { get => true; }

    public override bool IsCollidable { get => !this.IsOpened; }

    public override bool IsClimbable { get => this.IsOpened; }

    public override RectangleShape CollisionBox { get; set; } = new (new Vector2f(Block.Size, Block.Size / 5));

    public bool IsOpened { get; set; } = true;

    public override void Draw(RenderTarget target, RenderStates states)
    {
        if (this.Light > 0)
        {
            target.Draw(this.Sprite, states);
        }

        Drawable shadow = this.Chunk.WallMesh[this.Coord] is not EmptyWall ?
            new RectangleShape(this.CollisionBox)
            {
                FillColor = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - this.Light))),
                Position = new Vector2f(0, 0),
                Size = new Vector2f(Block.Size, Block.Size),
            }
            :
            new Sprite(this.Sprite)
            {
                Color = new Color(0, 0, 0, (byte)Math.Max(0, Math.Min(255, 255 - this.Light))),
            };

        target.Draw(shadow, states);
    }

    public override void OnClick()
    {
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

    public override Block Copy()
    => new Trapdoor()
    {
        CollisionBox = new RectangleShape(this.CollisionBox),
        Coord = this.Coord,
        Light = this.Light,
        WasUpdated = this.WasUpdated,
        IsOpened = this.IsOpened,
    };
}
